using CodingChallenge.ChallengeServer.Abstractions;

namespace CodingChallenge.ChallengeServer.Api.Services;

public class SampleChallenge02 : IChallengeService
{
    public string Identifier => "Sample_02";
    public string Name => "Sample 02";
    public string Description => "A very hard and dead serious challenge, I am not sure if anyone is going to make it";

    public IEnumerable<string> PreRequisits => new[] { "Sample_01" };

    public async Task<ChallengeData> GetChallengeAsync(string challengee)
    {
        return new("Return 2");
    }

   
    public async Task<ChallengeResult> TestChallengeAsync(string challengee, string input)
    {
        return new (input == "2", "");
    }
}