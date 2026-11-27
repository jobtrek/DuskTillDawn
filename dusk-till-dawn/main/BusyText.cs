// One wording for the time a wrong action costs, so the admin console and every repair prompt say the same thing.
public static class BusyText
{
    public static string For(double secondsLeft)
    {
        // Rounded up: the player is still locked during the last fraction of a second.
        return $"Wrong action: busy {System.Math.Ceiling(secondsLeft):0} s";
    }
}
