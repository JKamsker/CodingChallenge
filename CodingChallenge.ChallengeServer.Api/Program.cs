using CodingChallenge.ChallengeServer.Api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Web.Http;
using CodingChallenge.ChallengeServer.Abstractions;
using CodingChallenge.ChallengeServer.NumberMatrix;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services
#if DEBUG
    .AddTransient<IChallengeService, SampleChallenge>()
    .AddTransient<IChallengeService, SampleChallenge02>()
#endif
    .AddTransient<IChallengeService, NumberMatrix01>()
    ;

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


public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null)
        {
            return;
        }

        if (context.Exception is HttpResponseException httpResponseException)
        {
            context.Result = new ObjectResult(httpResponseException.Message)
            {
                StatusCode = (int)httpResponseException.Response.StatusCode
            };

            context.ExceptionHandled = true;
        }
        //else
        //{
        //    context.Result = new ObjectResult("Unknown error")
        //    {
        //        StatusCode = 500
        //    };

        //    context.ExceptionHandled = true;
        //}
    }
}