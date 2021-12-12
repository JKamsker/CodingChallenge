using AutoMapper;

using CodingChallenge.Discord.Bot.Models.ChallengeApi;

using Microsoft.Extensions.Logging;

using Rnd.Lib.Extensions;
using CodingChallenge.Discord.Bot.Dal;

namespace CodingChallenge.Discord.Bot.Services.Challenge
{
    public class ChallengeService
    {
        private readonly ChallengeRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ChallengeService> _logger;
        private readonly ChallengeServiceStorage _serviceStorage;
        private readonly UserDao _userDao;
        private readonly ChallengeDao _challengeDao;

        public ChallengeService
        (
            ChallengeRepositoryFactory repositoryFactory,
            IMapper mapper,
            ILogger<ChallengeService> logger,
            ChallengeServiceStorage serviceStorage,
            UserDao userDao,
            ChallengeDao challengeDao
        )
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _logger = logger;
            _serviceStorage = serviceStorage;
            _userDao = userDao;
            _challengeDao = challengeDao;
        }

        public async IAsyncEnumerable<ChallengeServiceStorageEntity> ListAsync(ulong userId)
        {
            var user = await _userDao.GetOrCreateUserAsync(userId);
            var challenges = _serviceStorage.ListAll();

            foreach (var challenge in challenges)
            {
                var meetsPrerequesits = challenge.ChallengeData.PreRequisits.OrEmpty()
                    .All(id => user.HasSolved(challenge.FullyQualifiedIdentifier, id));

                if (meetsPrerequesits && !user.HasSolved(challenge.FullyQualifiedIdentifier))
                {
                    yield return challenge;
                }
            }
        }

        public async Task<ChallengeDescriptionDto?> StartChallengeAsync(string challengeIdentifier, ulong challengee)
        {
            var fqId = await _serviceStorage.GetFullyQualifiedName(challengeIdentifier);
            var repo = await GetRepositoryAsync(fqId);
            if (repo is null)
            {
                return null;
            }

            var user = await _userDao.GetOrCreateUserAsync(challengee);
            if (user.StartChallenge(fqId))
            {
                await _userDao.PersistUserAsync(user);
            }

            await _challengeDao.SetStartedAt(fqId, DateTimeOffset.UtcNow);

            return await repo.GenerateChallengeAsync(fqId.Identifier, challengee.ToString());
        }

        public async Task<ChallengeSolutionResponseDto?> SolveChallengeAsync(ulong userId, ChallengeSolutionRequestDto solutionRequestDto)
        {
            solutionRequestDto.Challengee = userId.ToString();
            var repo = await GetRepositoryAsync(solutionRequestDto.ChallengeIdentifier);
            if (repo is null)
            {
                return null;
            }
            

            var fqn = await _serviceStorage.GetFullyQualifiedName(solutionRequestDto.ChallengeIdentifier);
            if (fqn == FullyQualifiedName.Default)
            {
                return null;
            }

            solutionRequestDto.ChallengeIdentifier = fqn.Identifier;

            var user = await _userDao.GetOrCreateUserAsync(userId);
            if (user.HasSolved(fqn.Repository, fqn.Identifier))
            {
                return new ChallengeSolutionResponseDto(false, "The challenge can only be solved once");
            }

            var solution = await repo.SolveChallengeAsync(solutionRequestDto);
            if (solution?.Success == true && user.SolveChallenge(fqn))
            {
                await _userDao.PersistUserAsync(user);
            }

            return solution;
        }

        private async Task<IChallengeRepository?> GetRepositoryAsync(string challengeIdentifier)
        {
            var identifier = await _serviceStorage.GetRepositoryDescriptor(challengeIdentifier);
            if (identifier == null)
            {
                return null;
            }

            return await _repositoryFactory.CreateRepositoryAsync(identifier);
        }
    }
}