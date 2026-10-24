namespace Example05.Presentation.Authentication;

public sealed record ApiKeyOptions
{
    public const string SectionName = "Authentication";

    public string ApiKey { get; init; } = string.Empty;
}
