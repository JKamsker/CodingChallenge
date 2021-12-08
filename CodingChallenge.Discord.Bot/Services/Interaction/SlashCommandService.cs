using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Diagnostics;

//using DiscordBot.Domain.Configuration;
//using DiscordBot.Modules.Utils.ReactionBase;
namespace CodingChallenge.Discord.Bot.InteractionServices;

public class SlashCommandService
{
    private readonly InteractionService _interactionService;
    private readonly DiscordSocketClient _client;
    private readonly ILogger<SlashCommandService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SlashCommandService
    (
        DiscordSocketClient discord,
        ILogger<SlashCommandService> logger,
        IServiceProvider serviceProvider
    )
    {
        _interactionService = new InteractionService(discord);
        _client = discord;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(System.Reflection.Assembly.GetExecutingAssembly(), _serviceProvider);
        await _interactionService.RegisterCommandsToGuildAsync(704990064039559238);
        

        _interactionService.AutocompleteCommandExecuted += async (a, b, c) =>
        {
            Debugger.Break();
        };

        _interactionService.AutocompleteHandlerExecuted += async (autocompleteHandler, interactionContext, result) =>
        {
            Debugger.Break();
        };

        //_client.SlashCommandExecuted += (e) => _interactionService.ExecuteCommandAsync(e.Data, _serviceProvider);
        //await SlashCommandAsync();
        _client.ButtonExecuted += async (interaction) =>
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };

        _client.SlashCommandExecuted += async (interaction) =>
        {
            var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };

        _client.AutocompleteExecuted += async (interaction) =>
        {
            var ctx = new SocketInteractionContext<SocketAutocompleteInteraction>(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        try
        {
            switch (command.Data.Name)
            {
                case "challenge":
                    await HandleChallenge(command);
                    break;

                default:
                    break;
            }
        }
        finally
        {
            if (!command.HasResponded)
            {
                await command.RespondAsync($"No response for {command.Data.Name}");
            }
        }
    }

    private async Task HandleChallenge(SocketSlashCommand command)
    {
        LogJson(command.Data);

        var options = command.Data.Options;

        var operation = options.FirstOrDefault();
        switch (operation?.Name)
        {
            case "list":
                {
                    var searchString = operation?.Options.FirstOrDefault()?.Value;
                    await command.RespondAsync($"Listing '{searchString}'");
                }
                break;

            case "start":
                {
                    var identifier = operation?.Options.FirstOrDefault()?.Value;
                    await command.RespondAsync($"Starting '{identifier}'");
                }
                break;

            case "solve":
                {
                    var identifier = operation?.Options.FirstOrDefault()?.Value;
                    var solution = operation?.Options.Skip(1).FirstOrDefault()?.Value;

                    await command.RespondAsync($"Solution for '{identifier}' is '{solution}'");
                }
                break;
        }
    }

    private async Task SlashCommandAsync()
    {
        var guild = _client.GetGuild(704990064039559238);

        //await guild.DeleteApplicationCommandsAsync();

        // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
        var guildCommand = new SlashCommandBuilder();

        guildCommand
            .WithName("challenge") // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            .WithDescription("Do something with challenges")   // Descriptions can have a max length of 100.
            .AddOptions
            (
                new SlashCommandOptionBuilder()
                    .WithName("list")
                    .WithDescription("Lists all challenges")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("search-string", ApplicationCommandOptionType.String, "Searches challenges based on title", required: false),

                new SlashCommandOptionBuilder()
                    .WithName("start")
                    .WithDescription("Starts a challenge")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("identifier", ApplicationCommandOptionType.String, "Starts a challenge based on the id", required: true),

                new SlashCommandOptionBuilder()
                    .WithName("solve")
                    .WithDescription("solves a challenge")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("identifier", ApplicationCommandOptionType.String, "Identifier for the challenge", required: true)
                    .AddOption("solution", ApplicationCommandOptionType.String, "Solution to the challenge", required: true)
            // .WithAutocomplete(true)
            )
            ;

        // Let's do our global command
        //var globalCommand = new SlashCommandBuilder();
        //globalCommand.WithName("first-global-command");
        //globalCommand.WithDescription("This is my frist global slash command");

        try
        {
            // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
            var command = await guild.CreateApplicationCommandAsync(guildCommand.Build());

            //command.Type == ApplicationCommandType.

            // With global commands we dont need the guild.
            //await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
            // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
        }
        catch (ApplicationCommandException exception)
        {
            LogJson(exception.Errors);
        }
    }

    private void LogJson(object @object)
    {
        // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
        var json = JsonConvert.SerializeObject(@object, Formatting.Indented);

        // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
        //Console.WriteLine(json);
        _logger.LogWarning(json);
    }
}