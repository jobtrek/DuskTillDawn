using System;

namespace DuskTillDawn.Core;

public enum MailKind
{
    Warning,
    News,
    Phishing,
}

public sealed class Mail
{
    public required int Id { get; init; }
    public required MailKind Kind { get; init; }
    public required string From { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }

    // The Incident a Warning mail announces; News and Phishing mails have none.
    public string? IncidentId { get; init; }

    public required TimeOnly ArrivedAt { get; init; }
    public bool IsRead { get; internal set; }
    public bool IsReported { get; internal set; }
}
