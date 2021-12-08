namespace CodingChallenge.Discord.Bot.Services.Challenge;

public class ChallengeRepositoryFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ChallengeRepositoryFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IChallengeRepository> CreateRepositoryAsync(IChallengeRepositoryDescriptor challengeRepositoryDescriptor) => challengeRepositoryDescriptor switch
    {
        HttpChallengeRepositoryDescriptor httpDescriptor => new HttpChallengeRepository(httpDescriptor, _httpClientFactory), //httpDescriptor
        InMemChallengeRepositoryDescriptor inMemDescriptor => new SampleChallengeRepository(),
        _ => throw new NotImplementedException()
    };
}