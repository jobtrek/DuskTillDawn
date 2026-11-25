namespace DuskTillDawn.Core;

// What the player sees at 08:00, or when the night is lost: how the shift went.
public sealed record NightReport(bool Survived, double SlaLeft, int Solved, int Failed, int PointsEarned)
{
    public static NightReport Of(GameState state, Game game)
    {
        bool survived = state.Status == NightStatus.Won && !game.IsLost;
        return new NightReport(survived, state.Sla, game.Solved, game.Failures, state.PointsEarnedThisNight);
    }
}
