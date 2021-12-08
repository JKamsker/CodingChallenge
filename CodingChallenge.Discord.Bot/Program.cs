using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CodingChallenge.Discord.Bot.Extensions;
using CodingChallenge.Discord.Bot;

var variables = Environment.GetEnvironmentVariables();
var ser = System.Text.Json.JsonSerializer.Serialize(variables);
Console.WriteLine($"Variables: {ser}");

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .UseStartup<Startup>()
    .Build().Run();