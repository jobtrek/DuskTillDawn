namespace DuskTillDawn.Core;

public enum PhishingStage
{
    Warning,
    Active,
    Escalated,
}

// A fake e-mail arrived. Fixed by reporting it from the control room.
// Ignored, it gets worse (docs/game-v1.md §7): no damage while it is a Warning, the SLA drains once it is
// Active, and the drain doubles once it is Escalated. Reported early, it pays the fast repair bonus.
public sealed class PhishingEmail(double durationSeconds = 300)
    : Challenge("New urgent message", Room.Control, points: 100, durationSeconds)
{
    // Warning for the first fifth of its time, Escalated for the last fifth: 60 s and 4 min of the usual 5 min,
    // so a shorter one (the demo run's Hard step) goes through every stage too.
    private const double ActiveFraction = 0.2;
    private const double EscalatedFraction = 0.8;
    private const double ActiveSlaDrainPerMinute = 0.04;

    public PhishingStage Stage => ((DurationSeconds - TimeRemaining) / DurationSeconds) switch
    {
        >= EscalatedFraction => PhishingStage.Escalated,
        >= ActiveFraction => PhishingStage.Active,
        _ => PhishingStage.Warning,
    };

    public override double SlaDrainPerMinute => Stage switch
    {
        PhishingStage.Escalated => ActiveSlaDrainPerMinute * 2,
        PhishingStage.Active => ActiveSlaDrainPerMinute,
        _ => 0,
    };

    // Reported in Warning it never drained anything, so there is no SLA to give back.
    public override double SlaRestoredOnSuccess => 0;

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.ReportEmail;
    }
}
