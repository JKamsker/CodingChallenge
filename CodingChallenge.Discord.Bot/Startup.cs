using System;
using System.IO;
using System.Net;
using System.Net.Http;

using Discord.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MongoDB.Driver;
using MongoDB.Bson.Serialization;

using Discord.WebSocket;
using CodingChallenge.Discord.Bot.InteractionServices;
using CodingChallenge.Discord.Bot.Services.Challenge;
using CodingChallenge.Discord.Bot.Models.Mongo;
using CodingChallenge.Discord.Bot.Models.Automapper;
using CodingChallenge.Discord.Bot.Dal;

using static CodingChallenge.Discord.Bot.InteractionServices.CommandHandlingService;
using System.Diagnostics;
using System.Security.Authentication;
using CodingChallenge.Discord.Bot.Extensions;
using CodingChallenge.Discord.Bot;
using Rnd.Lib.Utils;

namespace CodingChallenge.Discord.Bot;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddOptions()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<SlashCommandService>()
            .AddSingleton<CommandService, InjectableCommandService>()

            .AddTransient<ChallengeDao>()
            .AddTransient<UserDao>()

            .AddTransient<ChallengeService>()
            .AddScoped<IChallengeRepository, SampleChallengeRepository>()
            .AddSingleton<ChallengeServiceStorage>()
            .AddSingleton<ChallengeRepositoryFactory>()

            .AddHostedService<BotService>()
            .AddSingleton<DiscordSocketClient>()

            .AddHttpClient();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.Configure<DiscordSettings>(Configuration.GetSection("Discord"));
        services.AddAutoMapper(typeof(ChallengeProfile));

        ConfigureMongoDb(services);
    }

    private void ConfigureMongoDb(IServiceCollection services)
    {
        BsonClassMap.RegisterClassMap<InMemChallengeRepositoryDescriptor>();
        BsonClassMap.RegisterClassMap<HttpChallengeRepositoryDescriptor>();

        var databaseName = Configuration["Database:Name"]?.ToString();
        Console.WriteLine($"Database Name: {databaseName}");

        var mongoClient = CreateMongoClient();
        var db = mongoClient.GetDatabase(databaseName);

        IMongoCollection<DiscordUserData> users = db.GetCollection<DiscordUserData>("Users");
        IMongoCollection<ChallengeRepositoryData> repositoryData = db.GetCollection<ChallengeRepositoryData>("ChallengeRepositories");

        repositoryData.Indexes.CreateOne
        (
            new CreateIndexModel<ChallengeRepositoryData>
            (
                Builders<ChallengeRepositoryData>.IndexKeys.Ascending(item => item.CreatedAt)
            )
        );

        services
            .AddSingleton(db)
            .AddSingleton(users)
            .AddSingleton(repositoryData)
            ;

        var overwriteRepository = Configuration["OVERWRITE_REPOSITORY"]?.ToString();
        var defaultRepository = Configuration["DEFAULT_REPOSITORY"]?.ToString();

        (bool Parsed, bool Overwrite, string Value) action =
            UriEx.IsValidUrl(overwriteRepository)
            ? (true, true, overwriteRepository)
            : UriEx.IsValidUrl(defaultRepository)
                ? (true, false, defaultRepository)
                : (false, false, String.Empty);

        if (action.Parsed)
        {
            if (action.Overwrite)
            {
                repositoryData.DeleteMany(x => true);
            }

            if (repositoryData.EstimatedDocumentCount() == 0)
            {
                repositoryData.InsertMany(new List<ChallengeRepositoryData>
                {
                    new ChallengeRepositoryData()
                    {
                        RepositoryName = "SampleChallenge",
                        RepositoryDescriptor = new HttpChallengeRepositoryDescriptor { BaseUrl = action.Value },
                        CreatedAt =  DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    },
                });
            }
        }
    }

    private MongoClient CreateMongoClient()
    {
        var connectionString = Configuration["Database:ConnectionString"]?.ToString();
        Console.WriteLine($"connectionString: {connectionString}");

        var settings = MongoClientSettings.FromUrl(
            new MongoUrl(connectionString)
        );

        settings.SslSettings = new() { EnabledSslProtocols = SslProtocols.Tls12 };

        return new MongoClient(settings);
    }
}