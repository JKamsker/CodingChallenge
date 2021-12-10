using AutoMapper;

using CodingChallenge.Discord.Bot.Dal;
using CodingChallenge.Discord.Bot.Models.ChallengeApi;

using Microsoft.Extensions.Logging;

namespace CodingChallenge.Discord.Bot.Services.Challenge
{
    public class ChallengeServiceStorage
    {
        private readonly ChallengeRepositoryFactory _repositoryFactory;
        private readonly ChallengeDao _persistenceService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChallengeServiceStorage> _logger;

        public ChallengeServiceStorage
        (
            ChallengeRepositoryFactory repositoryFactory,
            ChallengeDao persistenceService,
            IMapper mapper,
            ILogger<ChallengeServiceStorage> logger
        )
        {
            _repositoryFactory = repositoryFactory;
            _persistenceService = persistenceService;
            _mapper = mapper;
            _logger = logger;
        }

        private Dictionary<string, ChallengeServiceStorageEntity> _identifiers;
        private List<ChallengeServiceStorageEntity> _descriptors;

        public async Task InitializeAsync()
        {
            Dictionary<string, ChallengeServiceStorageEntity> repositoriesByChallengeIdentifier = new();
            List<ChallengeServiceStorageEntity> descriptors = new();

            await foreach (var repositoryData in _persistenceService.EnumerateRepositoryData())
            {
                var repository = await _repositoryFactory.CreateRepositoryAsync(repositoryData.RepositoryDescriptor);
                var discovery = await repository.GetDiscoveryAsync();

                if (discovery == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(discovery.RepositoryName)
                    && !string.Equals(repositoryData.RepositoryName, discovery.RepositoryName))
                {
                    repositoryData.RepositoryName = discovery.RepositoryName;
                    await _persistenceService.UpsertRepositoryData(repositoryData);
                }

                foreach (var challenge in discovery.Challenges)
                {
                    var simpleName = challenge.Identifier;
                    var fullyQualifiedName = $"{repositoryData.RepositoryName}.{challenge.Identifier}";

                    var descriptor = new ChallengeServiceStorageEntity(
                        Identifier: simpleName,
                        RepositoryName: repositoryData.RepositoryName,
                        Descriptor: repositoryData.RepositoryDescriptor,
                        ChallengeData: challenge
                    );

                    if (!repositoriesByChallengeIdentifier.TryAdd(fullyQualifiedName, descriptor))
                    {
                        _logger.LogError($"Cannot register '{fullyQualifiedName}': Key already in dictionary");
                        continue;
                    }

                    if (!repositoriesByChallengeIdentifier.TryAdd(simpleName, descriptor))
                    {
                        _logger.LogInformation($"Cannot register simple '{simpleName}': Key already in dictionary");
                        descriptor = descriptor with { IsIdentifierUnique = false };
                    }

                    descriptors.Add(descriptor);
                }
            }

            _identifiers = repositoriesByChallengeIdentifier;
            _descriptors = descriptors;
        }

        public async Task<IChallengeRepositoryDescriptor?> GetRepositoryDescriptor(string identifier)
        {
            if (_identifiers.TryGetValue(identifier, out var descriptor))
            {
                return descriptor?.Descriptor;
            }
            return null;
        }

        public async Task<FullyQualifiedName> GetFullyQualifiedName(string identifier)
        {
            if (_identifiers.TryGetValue(identifier, out var descriptor))
            {
                return descriptor?.FullyQualifiedIdentifier ?? FullyQualifiedName.Default;
            }
            return FullyQualifiedName.Default;
        }

        public IEnumerable<ChallengeServiceStorageEntity> ListAll() => _descriptors.AsReadOnly();
    }
}