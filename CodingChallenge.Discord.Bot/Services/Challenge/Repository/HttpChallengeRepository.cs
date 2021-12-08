using System.Net.Http.Headers;
using System.Net.Http.Json;

using CodingChallenge.Discord.Bot.Models.ChallengeApi;

namespace CodingChallenge.Discord.Bot.Services.Challenge;

public class HttpChallengeRepository : IChallengeRepository
{
    private readonly HttpChallengeRepositoryDescriptor _httpDescriptor;
    private readonly IHttpClientFactory _clientFactory;

    public HttpChallengeRepository
    (
        HttpChallengeRepositoryDescriptor httpDescriptor,
        IHttpClientFactory clientFactory
    )
    {
        _httpDescriptor = httpDescriptor;
        _clientFactory = clientFactory;
    }

    public async Task<ChallengeDiscoveryDto?> GetDiscoveryAsync()
    {
        var client = GetHttpClient();
        using var request = await client.GetAsync("/api/v1/Challenge/Discovery");
        request.EnsureSuccessStatusCode();
        return await request.Content.ReadFromJsonAsync<ChallengeDiscoveryDto>();
    }

    public async Task<ChallengeDescriptionDto?> GenerateChallengeAsync(string challengeIdentifier, string challengee)
    {
        var client = GetHttpClient();
        using var request = await client.GetAsync($"/api/v1/Challenge/{challengeIdentifier}/challengee/{challengee}");
        request.EnsureSuccessStatusCode();
        return await request.Content.ReadFromJsonAsync<ChallengeDescriptionDto>();
    }

    public async Task<ChallengeSolutionResponseDto?> SolveChallengeAsync(ChallengeSolutionRequestDto solutionRequestDto)
    {
        var client = GetHttpClient();
        using var request = await client.PostAsJsonAsync
        (
            $"/api/v1/Challenge/{solutionRequestDto.ChallengeIdentifier}/challengee/{solutionRequestDto.Challengee}",
            solutionRequestDto
        );

        request.EnsureSuccessStatusCode();
        return await request.Content.ReadFromJsonAsync<ChallengeSolutionResponseDto>();
    }

    private HttpClient GetHttpClient()
    {
        var url = new Uri(_httpDescriptor.BaseUrl);
        var client = _clientFactory.CreateClient(url.Host);
        client.BaseAddress = url;
        return client;
    }
}