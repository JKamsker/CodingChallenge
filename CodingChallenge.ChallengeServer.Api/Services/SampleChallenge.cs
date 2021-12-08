using CodingChallenge.ChallengeServer.Abstractions;

namespace CodingChallenge.ChallengeServer.Api.Services
{
    public class SampleChallenge : IChallengeService
    {
        public string Identifier => "Sample_01";
        public string Name => "Sample 01";
        public string Description => "A very hard and dead serious challenge, I am not sure if anyone is going to make it";

        public IEnumerable<string> PreRequisits => Enumerable.Empty<string>();

        public async Task<ChallengeData> GetChallengeAsync(string challengee)
        {
            return new ChallengeData("Oi amigos");
        }

      
        public async Task<ChallengeResult> TestChallengeAsync(string challengee, string input)
        {
            return new(input == "1", "");
        }
    }
}