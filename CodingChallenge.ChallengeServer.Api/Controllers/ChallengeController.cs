using CodingChallenge.ChallengeServer.Abstractions;
using CodingChallenge.ChallengeServer.Api.Models;
using CodingChallenge.ChallengeServer.Api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using ChallengeResult = CodingChallenge.ChallengeServer.Abstractions.ChallengeResult;

namespace CodingChallenge.ChallengeServer.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ChallengeController : ControllerBase
{
    private readonly IEnumerable<IChallengeService> _challengeServices;
    private readonly ILogger<ChallengeController> _logger;
    private readonly ChallengeServerConfiguration _options;

    public ChallengeController
    (
        IEnumerable<IChallengeService> challengeServices,
        ILogger<ChallengeController> logger,
        IOptions<ChallengeServerConfiguration> options
    )
    {
        _challengeServices = challengeServices;
        _logger = logger;
        _options = options.Value;
    }

    [HttpGet("Discovery")]
    public object Get() => new
    {
        RepositoryName = _options.RepositoryName ?? "DemoRepo",
        Challenges = _challengeServices.Select(x => new
        {
            x.Identifier,
            x.Name,
            x.Description,
            x.PreRequisits,
        })
    };

    // api/v1/challenge/sample_01/challengee/0123456
    [HttpGet("{challengeIdentifier}/challengee/{challengee}")]
    public async Task<object> GenerateChallenge(string challengeIdentifier, string challengee)
    {
        var challenge = _challengeServices
            .FirstOrDefault(x => x.Identifier == challengeIdentifier);

        if (challenge == null)
        {
            return new NotFoundObjectResult($"Challenge '{challengeIdentifier}' was not found");
        }

        return await challenge.GetChallengeAsync(challengee);
    }

    [HttpPost("{challengeIdentifier}/challengee/{challengee}")]
    public async Task<ChallengeResult> SolveChallengeForChallengee
    (
        string challengeIdentifier,
        string challengee,
        [FromBody] ChallengeResultDto challengeResult
    )
    {
        var challenge = _challengeServices
            .FirstOrDefault(x => x.Identifier == challengeIdentifier);

        if (challenge == null)
        {
            throw new ArgumentException($"Challenge '{challengeIdentifier}' was not found");
        }

        return await challenge.TestChallengeAsync(challengee, challengeResult.Result);
    }
}

public record ChallengeResultDto(string Result);