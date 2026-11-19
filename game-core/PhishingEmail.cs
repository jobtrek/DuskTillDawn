namespace DuskTillDawn.Core;

// A fake e-mail arrived. Fixed by reporting it from the control room.
public sealed class PhishingEmail() : Challenge("New urgent message", Room.Control, points: 100, durationSeconds: 60)
{
    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.ReportEmail;
    }
}
