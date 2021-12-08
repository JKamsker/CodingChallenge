using CodingChallenge.Discord.Bot.Services.Challenge;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodingChallenge.Discord.Bot.Models.Mongo;

public class DiscordUserData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ulong DiscordUserId { get; set; }

    public List<SolvedChallenge> SolvedChallenges { get; set; } = new();

    public bool HasSolved(string repositoryId, string challengeId)
    {
        return SolvedChallenges.Any(x => x.RepositoryId == repositoryId && x.ChallengeId == challengeId);
    }

    public bool HasSolved(FullyQualifiedName fqn)
        => HasSolved(fqn.Repository, fqn.Identifier);
}

public class SolvedChallenge
{
    public string RepositoryId { get; set; }
    public string ChallengeId { get; set; }

    public DateTimeOffset? SolvedAt { get; set; }
}

public class ChallengeRepositoryData
{
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// RepositoryName found in the Discovery
    /// </summary>
    public string RepositoryName { get; set; }

    public IChallengeRepositoryDescriptor RepositoryDescriptor { get; set; }

    public long CreatedAt { get; set; }

    //public ListAsync<ChallengeData> Challenges { get; set; }
}