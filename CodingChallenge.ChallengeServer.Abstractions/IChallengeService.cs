//namespace CodingChallenge.ChallengeServer.Abstractions;
namespace CodingChallenge.ChallengeServer.Abstractions;

public record ChallengeResult(bool Success, string Message);

public record ChallengeData(string Message, string[] attachments = null);

public interface IChallengeService
{
    string Identifier { get; }
    string Name { get; }
    string Description { get; }
    IEnumerable<string> PreRequisits { get; }

    Task<ChallengeData> GetChallengeAsync(string challengee);

    Task<ChallengeResult> TestChallengeAsync(string challengee, string input);
}