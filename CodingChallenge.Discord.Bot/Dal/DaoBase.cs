
using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Discord.Bot.Dal;

public class DaoBase<TDocument>
{
    protected readonly IMongoCollection<TDocument> Collection;

    public DaoBase(IMongoCollection<TDocument> collection)
    {
        Collection = collection;
    }

    protected FilterDefinitionBuilder<TDocument> Filter => Builders<TDocument>.Filter;
    protected IndexKeysDefinitionBuilder<TDocument> IndexKeys => Builders<TDocument>.IndexKeys;
    protected ProjectionDefinitionBuilder<TDocument> Projection => Builders<TDocument>.Projection;
    protected SortDefinitionBuilder<TDocument> Sort => Builders<TDocument>.Sort;
    protected UpdateDefinitionBuilder<TDocument> Update => Builders<TDocument>.Update;
}
