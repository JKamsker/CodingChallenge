using CodingChallenge.Discord.Bot.Models.Mongo;

using MongoDB.Driver;

using Rnd.MongoDb;

namespace CodingChallenge.Discord.Bot.Dal;

public class UserDao : DaoBase<DiscordUserData>
{
    public UserDao(IMongoCollection<DiscordUserData> collection) : base(collection)
    {
    }

    public async Task<DiscordUserData> GetOrCreateUserAsync(ulong discordId)
    {
        var user = await Collection
            .FindAsync(x => x.DiscordUserId == discordId)
            .EnumerateAsync()
            .FirstOrDefaultAsync();

        if (user == null)
        {
            await Collection.InsertOneAsync(user = new DiscordUserData
            {
                DiscordUserId = discordId
            });
        }

        return user;
    }

    internal async Task PersistUserAsync(DiscordUserData user)
    {
        await Collection.FindOneAndReplaceAsync
        (
             Filter.Where(x => x.Id == user.Id),
             user
        );
    }
}
