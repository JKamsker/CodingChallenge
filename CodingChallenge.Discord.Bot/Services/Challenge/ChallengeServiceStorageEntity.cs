using CodingChallenge.Discord.Bot.Models.ChallengeApi;

using MongoDB.Driver;

namespace CodingChallenge.Discord.Bot.Services.Challenge
{
    public record ChallengeServiceStorageEntity
    (
        string Identifier,
        string RepositoryName,
        IChallengeRepositoryDescriptor Descriptor,
        ChallengeDto ChallengeData,
        bool IsIdentifierUnique = true
    )
    {
        public FullyQualifiedName FullyQualifiedIdentifier
            => new(RepositoryName, Identifier);

        public string RenderPreRequisits()
        {
            if (!(ChallengeData?.PreRequisits ?? Enumerable.Empty<string>()).Any())
            {
                return String.Empty;
            }
            return $"*{string.Join(';', ChallengeData.PreRequisits)}*";
        }

        public string GetPrimaryIdentifier()
            => string.IsNullOrEmpty(Identifier) ? FullyQualifiedIdentifier : Identifier;
        public string RenderIdentifier()
        {
            return IsIdentifierUnique
                ? $"*{Identifier}* (`{FullyQualifiedIdentifier}`)"
                : $"*{FullyQualifiedIdentifier}*";
        }
    }

    public record FullyQualifiedName(string Repository, string Identifier)
    {
        public static FullyQualifiedName Default => new(string.Empty, string.Empty);

        public override string ToString()
            => $"{Repository}.{Identifier}";

        public static implicit operator string(FullyQualifiedName fqn) => fqn.ToString();
    }
}