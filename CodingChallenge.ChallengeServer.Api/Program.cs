using CodingChallenge.ChallengeServer.Api.Services;
using CodingChallenge.ChallengeServer.Abstractions;

//using CodingChallenge.ChallengeServer.NumberMatrix;
using Rnd.IO.Extensions;
using CodingChallenge.ChallengeServer.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<ChallengeServerConfiguration>(builder.Configuration.GetSection("ChallengeServer"));

var pluginPath = builder.Configuration["ChallengeServer:PluginDir"]?.ToString();
var pluginDir = string.IsNullOrEmpty(pluginPath)
    ? Environment.CurrentDirectory.AsDirectoryInfo().GetDirectory("Plugins")
    : pluginPath.AsDirectoryInfo();

new PluginLoader(pluginDir)
    .LoadPlugins(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();