using CodingChallenge.Discord.Bot.Models.ChallengeApi;

namespace CodingChallenge.Discord.Bot.Services.Challenge
{
    public class SampleChallengeRepository : IChallengeRepository
    {
        public async Task<ChallengeDiscoveryDto?> GetDiscoveryAsync() => new ChallengeDiscoveryDto
        {
            RepositoryName = "In-Memory",
            Challenges = new List<ChallengeDto>
            {
                new()
                {
                    Identifier = "InMem_01",
                    Name = "InMemory 01",
                    Description = "A very hard and dead serious challenge, I am not sure if anyone is going to make it"
                },
                new()
                {
                    Identifier = "InMem_02",
                    Name = "InMemory 02",
                    Description = "01 A very hard and dead serious challenge, I am not sure if anyone is going to make it",
                    PreRequisits = new List<string>
                    {
                        "InMem_01"
                    }
                },
                new()
                {
                    Identifier = "InMem_03",
                    Name = "InMemory 03",
                    Description = "02 A very hard and dead serious challenge, I am not sure if anyone is going to make it",
                    PreRequisits = new List<string>
                    {
                        "InMem_02"
                    }
                },
            }
        };

        public async Task<ChallengeDescriptionDto?> GenerateChallengeAsync(string challengeIdentifier, string challengee) => new ChallengeDescriptionDto
        {
            Message = challengeIdentifier switch
            {
                "InMem_01" => "Return 01",
                "InMem_02" => "Return 02",
                "InMem_03" => "Return 03",
                _ => throw new NotImplementedException()
            },
        };

        public async Task<ChallengeSolutionResponseDto?> SolveChallengeAsync(ChallengeSolutionRequestDto solutionRequestDto)
            => new ChallengeSolutionResponseDto(solutionRequestDto.Result == solutionRequestDto.ChallengeIdentifier switch
            {
                "InMem_01" => "01",
                "InMem_02" => "02",
                "InMem_03" => "03",
                _ => throw new NotImplementedException()
            });
    }
}