using CodingChallenge.ChallengeServer.Abstractions;

namespace CodingChallenge.ChallengeServer.Plugins.Demo
{
    public class Demo02Challenge : IChallengeService
    {
        public string Identifier => "Demo_02";
        public string Name => "Demo 02";
        public string Description => "A very hard and dead serious challenge, I am not sure if anyone is going to make it";

        public IEnumerable<string> PreRequisits => new[] { "Demo_01" };

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