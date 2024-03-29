﻿using CodingChallenge.Discord.Bot.Models.ChallengeApi;

namespace CodingChallenge.Discord.Bot.Services.Challenge
{
    public interface IChallengeRepository
    {
        IChallengeRepositoryDescriptor Descriptor { get; }

        Task<ChallengeDiscoveryDto?> GetDiscoveryAsync();

        Task<ChallengeDescriptionDto?> GenerateChallengeAsync(string challengeIdentifier, string challengee);

        Task<ChallengeSolutionResponseDto?> SolveChallengeAsync(ChallengeSolutionRequestDto solutionRequestDto);
    }
}