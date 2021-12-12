using CodingChallenge.Discord.Bot.Services.Challenge;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodingChallenge.Discord.Bot.Models.Mongo;

public class DiscordUserData
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ulong DiscordUserId { get; set; }

    public List<UserChallengeState> ChallengeState { get; set; } = new();

    public bool HasSolved(FullyQualifiedName fqId)
        => HasSolved(fqId.Repository, fqId.Identifier);

    public bool HasSolved(string repositoryId, string challengeId)
    {
        return ChallengeState.Any(x => x.RepositoryId == repositoryId && x.ChallengeId == challengeId && x.SolvedAt != null);
    }

    public bool StartChallenge(FullyQualifiedName fqId)
    {
        var state = ChallengeState.FirstOrDefault(x => x.RepositoryId == fqId.Repository && x.ChallengeId == fqId.Identifier);
        if (state == null)
        {
            ChallengeState.Add(state = new()
            {
                RepositoryId = fqId.Repository,
                ChallengeId = fqId.Identifier,
                StartedAt = DateTimeOffset.UtcNow,
            });
            return true;
        }

        if (state.StartedAt == null)
        {
            state.StartedAt = DateTimeOffset.UtcNow;
            return true;
        }

        return false;
    }

    public bool SolveChallenge(FullyQualifiedName fqId)
    {
        var state = ChallengeState.FirstOrDefault(x => x.RepositoryId == fqId.Repository && x.ChallengeId == fqId.Identifier);
        if (state == null)
        {
            ChallengeState.Add(state = new()
            {
                RepositoryId = fqId.Repository,
                ChallengeId = fqId.Identifier,
                StartedAt = DateTimeOffset.UtcNow,
                SolvedAt = DateTimeOffset.UtcNow
            });
            return true;
        }

        if (state.SolvedAt == null)
        {
            state.SolvedAt = DateTimeOffset.UtcNow;
            return true;
        }

        return false;
    }
}

public class UserChallengeState
{
    public string RepositoryId { get; set; }
    public string ChallengeId { get; set; }

    public DateTimeOffset? StartedAt { get; set; }
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

    public List<ChallengeRepositoryChallengeData> Challenges { get; set; } = new();

    public ChallengeRepositoryChallengeData FindChallenge(FullyQualifiedName fullyQualifiedName)
    {
        Challenges ??= new();
        var challenge = Challenges.FirstOrDefault(x => x.Identifier == fullyQualifiedName.Identifier);
        if (challenge == null)
        {
            Challenges.Add(challenge = new()
            {
                Identifier = fullyQualifiedName.Identifier,
            });
        }
        return challenge;
    }
}

public class ChallengeRepositoryChallengeData
{
    public string Identifier { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
}