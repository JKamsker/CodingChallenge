using CodingChallenge.Discord.Bot.Models.Mongo;

using MongoDB.Bson;
using MongoDB.Driver;

using Rnd.MongoDb;

namespace CodingChallenge.Discord.Bot.Dal;

public class ChallengeDao : DaoBase<ChallengeRepositoryData>
{
    public ChallengeDao(IMongoCollection<ChallengeRepositoryData> challengeData)
        : base(challengeData)
    {
    }

    public async Task<ChallengeRepositoryData?> FindRepositoryDataByName(string repositoryName)
    {
        return await Collection
            .FindAsync(x => x.RepositoryName == repositoryName)
            .EnumerateAsync()
            .FirstOrDefaultAsync();
    }

    public IAsyncEnumerable<ChallengeRepositoryData> EnumerateRepositoryData()
    {
        return Collection.FindAsync(x => true, new()
        {
            Sort = Sort.Ascending(x => x.CreatedAt)
        })
        .EnumerateAsync();
    }

    public async Task UpsertRepositoryData(ChallengeRepositoryData repositoryData)
    {
        _ = string.IsNullOrEmpty(repositoryData.RepositoryName) ? throw new ArgumentException("RepositoryName cannot be empty!") : string.Empty;

        var filter = (repositoryData.Id == null || repositoryData.Id == ObjectId.Empty)
            ? Filter.Where(m => m.RepositoryName == repositoryData.RepositoryName)
            : Filter.Where(m => m.Id == repositoryData.Id);

        await Collection.ReplaceOneAsync
        (
            filter: filter,
            options: new ReplaceOptions { IsUpsert = true },
            replacement: repositoryData
        );
    }
}