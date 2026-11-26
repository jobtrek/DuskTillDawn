using DuskTillDawn.Core.Building;

namespace DuskTillDawn.Core.Incidents;

public enum PhishingStage
{
    Warning,
    Active,
    Escalated,
}

// A fake e-mail arrived. Fixed by reporting it from the control room.
// Ignored, it gets worse (docs/game-v1.md §7): no damage while it is a Warning, the SLA drains once it is
// Active, and the drain doubles once it is Escalated. Reported early, it pays the fast repair bonus.
public sealed class PhishingEmail() : Incident("New urgent message", Room.Control, points: 100, durationSeconds: 300)
{
    public const double ActiveAfterSeconds = 60;
    public const double EscalatedAfterSeconds = ActiveAfterSeconds + 180;
    private const double ActiveSlaDrainPerMinute = 0.04;

    public PhishingStage Stage => (DurationSeconds - TimeRemaining) switch
    {
        >= EscalatedAfterSeconds => PhishingStage.Escalated,
        >= ActiveAfterSeconds => PhishingStage.Active,
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
