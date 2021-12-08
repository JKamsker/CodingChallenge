//using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodingChallenge.Discord.Bot.Models.ChallengeApi;

public partial class ChallengeDescriptionDto
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("attachments")]
    public string[] Attachments { get; set; }
}

public partial class ChallengeSolutionRequestDto
{
    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonIgnore]
    public string ChallengeIdentifier { get; set; }

    [JsonIgnore]
    public string Challengee { get; set; }

    public ChallengeSolutionRequestDto()
    {
    }
}

public partial class ChallengeSolutionResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public ChallengeSolutionResponseDto()
    {
    }

    public ChallengeSolutionResponseDto(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }
}

/// <summary>
/// Response of
/// GET {{baseUrl}}/api/v1/Challenge/Discovery
/// </summary>
public partial class ChallengeDiscoveryDto
{
    [JsonPropertyName("repositoryName")]
    public string RepositoryName { get; set; }

    [JsonPropertyName("challenges")]
    public List<ChallengeDto> Challenges { get; set; }
}

public partial class ChallengeDto
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("preRequisits")]
    public List<string> PreRequisits { get; set; }
}