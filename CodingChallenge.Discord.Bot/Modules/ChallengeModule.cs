﻿using CodingChallenge.Discord.Bot.Models.ChallengeApi;
using CodingChallenge.Discord.Bot.Services.Challenge;

using Discord;

//using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using System.Linq;
using System.Text;

//using global::System.Linq.Async;

//using linqasync::System.Linq;

namespace CodingChallenge.Discord.Bot.Modules
{
    [Group("challenge", "Challenge the world!")]
    public class ChallengeModule : InteractionModuleBase
    {
        private readonly ChallengeService _challengeService;
        private readonly ChallengeServiceStorage _serviceStorage;

        public ChallengeModule
        (
            ChallengeService challengeService,
            ChallengeServiceStorage serviceStorage
        )
        {
            _challengeService = challengeService;
            _serviceStorage = serviceStorage;
        }

        [SlashCommand("list", "ListAsync a challenge")]
        public async Task ListAsync() //string searchString = ""
        {
            var embeds = _challengeService
                .ListAsync(base.Context.User.Id)
                //.SelectMany(x => x.Challenges.ToAsyncEnumerable())
                .Select(x => new EmbedFieldBuilder
                {
                    Name = x.ChallengeData.Name,
                    Value = $"Id: {x.RenderIdentifier()}\n" +
                            $"Name: *{x.ChallengeData.Name}*\n" +
                            $"Description: *{x.ChallengeData.Description}*\n" +
                            $"Prerequisits: {x.RenderPreRequisits()}"
                })
                ;

            var eb = new EmbedBuilder();
            await foreach (var embed in embeds)
            {
                eb.AddField(embed);
            }

            await this.Context.Interaction.RespondAsync(embed: eb.Build());
            var response = await this.Context.Interaction.GetOriginalResponseAsync();
        }

        [SlashCommand("start", "Start a new challenge!")]
        public async Task StartAsync(string identifier)
        {
            var challenge = await _challengeService.StartChallengeAsync(identifier, Context.User.Id);
            if (challenge is null)
            {
                await RespondAsync("`Sorry, i can't find that challenge`");
                return;
            }

            var challengeText = string.IsNullOrWhiteSpace(challenge.Message)
                ? "The challenge has no Text, sorry :("
                : challenge.Message;

            await RespondAsync(challengeText, ephemeral: true);

            if (challenge.Attachments?.Length is not null and > 0)
            {
                var streams = challenge.Attachments
                    .Select(x => Encoding.UTF8.GetBytes(x))
                    .Select(x => new MemoryStream(x))
                    .Select((x, i) => new FileAttachment(x, i > 1 ? $"ChallengeAttachment-{i}.txt" : "ChallengeAttachment.txt"))
                    .ToList();

                await Context.Channel.SendFilesAsync(streams);
            }

            //await Context.Channel.SendFileAsync(Stream.Null, "Info.Text");
        }

        [SlashCommand("solve", "Send your solution!")]
        public async Task SolveAsync(string identifier, string solution)
        {
            var challengeResult = await _challengeService.SolveChallengeAsync(Context.User.Id, new ChallengeSolutionRequestDto
            {
                ChallengeIdentifier = identifier,
                Result = solution
            });

            var message = challengeResult.Success ? "Success!" : "No success.";
            if (!string.IsNullOrEmpty(challengeResult.Message))
            {
                message += $"\n{challengeResult.Message}";
            }

            await this.Context.Interaction.RespondAsync(message);
        }

        //[AutocompleteCommand("identifier", "challenge start")]
        //public async Task Autocomplete()
        //{
        //    ListAsync<AutocompleteResult> results = new ListAsync<AutocompleteResult>();

        //    results.Add(new AutocompleteResult("identifier", "ayay"));
        //    results.Add(new AutocompleteResult("identifier", "xA"));

        //    if (Context.Interaction is SocketAutocompleteInteraction sai)
        //    {
        //        await sai.RespondAsync(results);
        //    }
        //}
    }
}