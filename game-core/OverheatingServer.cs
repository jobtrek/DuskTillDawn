namespace DuskTillDawn.Core;

// A server is too hot. Fixed in steps: restart the cooling in the server room, then wait for the room to recover.
public sealed class OverheatingServer() : Challenge("Temperature too high", Room.Servers, points: 100, durationSeconds: 75)
{
    public override WarningText Warning => new("Monitoring", "Server room overheating",
        "Temperature is climbing fast. Check the Heat on Monitoring.");

    protected override bool IsRightAction(PlayerAction action)
    {
        return action == PlayerAction.RestartCooling;
    }

    // Restarting the AC is only the first step (docs/game-v1.md §8): the room must cool below 32 °C and the
    // servers must be running again, rebooted at the rack after an Auto-shutdown.
    protected override bool NeedsRecovery => true;

    // With an empty battery the servers cannot be rebooted before the sun is back: the cooling part is done,
    // and the servers being off already drains the SLA.
    protected override bool IsRecovered(ServerRoom room)
    {
        bool onlyWaitingForPower = !room.ServersServing && room.Battery <= 0;
        return room.Heat < ServerRoom.ThrottleHeat && (room.ServersServing || onlyWaitingForPower);
    }
}
