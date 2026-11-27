namespace DuskTillDawn.Core.Tests;

// The cooling failure is fixed in steps (docs/player-flow/challenges.md, Cooling failure steps):
// restart the AC, wait below 32 °C, reboot the servers if they shut down.
public class CoolingStepsTests
{
    private const double NightHour = 22;

    private static (SoloGame Game, ServerRoom Room, OverheatingServer Incident) StartCoolingFailure()
    {
        var game = new SoloGame(new PlayerProfile("Bilal"));
        var room = new ServerRoom();
        var incident = new OverheatingServer();
        game.Launch(incident);
        return (game, room, incident);
    }

    // Same order as Main: the room updates its heat and servers, then the game finishes Recovering Incidents.
    private static void TickRoom(SoloGame game, ServerRoom room, double seconds)
    {
        room.Tick(seconds, NightHour, game);
        game.FinishRecovered(room);
    }

    [Fact]
    public void Restarting_the_cooling_does_not_resolve_the_incident_yet()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 40);

        bool fixedIt = game.Repair(game.Players[0], PlayerAction.RestartCooling);

        Assert.True(fixedIt);
        Assert.Equal(ChallengeState.Recovering, incident.State);
        Assert.Equal(0, game.Players[0].Points);
        Assert.False(game.Players[0].IsBusy);
    }

    [Fact]
    public void A_recovering_incident_stops_its_timer()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 40);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);
        double timeLeft = incident.TimeRemaining;

        game.Tick(1000);

        Assert.Equal(ChallengeState.Recovering, incident.State);
        Assert.Equal(timeLeft, incident.TimeRemaining);
        Assert.Equal(0, game.Failures);
    }

    [Fact]
    public void It_stays_recovering_while_the_room_is_still_too_hot()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 40);
        Assert.True(room.Heat >= ServerRoom.ThrottleHeat);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        TickRoom(game, room, 1);

        Assert.True(room.Heat >= ServerRoom.ThrottleHeat);
        Assert.Equal(ChallengeState.Recovering, incident.State);
    }

    [Fact]
    public void It_resolves_once_the_room_is_below_32_degrees_with_the_servers_running()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 40);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        TickRoom(game, room, 100);

        Assert.True(room.Heat < ServerRoom.ThrottleHeat);
        Assert.Equal(ChallengeState.Succeeded, incident.State);
        Assert.Equal(1, game.Solved);
    }

    [Fact]
    public void After_an_auto_shutdown_it_waits_for_the_servers_to_be_rebooted()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 100);
        Assert.Equal(ServerStatus.ShutDownByHeat, room.Status);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        TickRoom(game, room, 100);
        Assert.True(room.Heat < ServerRoom.ThrottleHeat);
        Assert.Equal(ChallengeState.Recovering, incident.State);

        room.TryReboot();
        TickRoom(game, room, ServerRoom.CleanRebootSeconds);

        Assert.Equal(ServerStatus.Running, room.Status);
        Assert.Equal(ChallengeState.Succeeded, incident.State);
    }

    [Fact]
    public void With_an_empty_battery_it_resolves_once_the_room_is_cool_without_a_reboot()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 1000);
        Assert.Equal(ServerStatus.ShutDownByHeat, room.Status);
        Assert.Equal(0, room.Battery);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        TickRoom(game, room, 100);

        Assert.True(room.Heat < ServerRoom.ThrottleHeat);
        Assert.False(room.CanReboot);
        Assert.Equal(ChallengeState.Succeeded, incident.State);
    }

    [Fact]
    public void The_fast_bonus_counts_from_the_time_left_when_the_cooling_was_restarted()
    {
        var (game, room, _) = StartCoolingFailure();
        TickRoom(game, room, 40);
        game.Tick(10);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        game.Tick(1000);
        TickRoom(game, room, 100);

        Assert.Equal(150, game.Players[0].Points);
    }

    [Fact]
    public void After_the_game_is_lost_a_recovering_incident_pays_nothing()
    {
        var (game, room, incident) = StartCoolingFailure();
        TickRoom(game, room, 40);
        game.Repair(game.Players[0], PlayerAction.RestartCooling);
        for (var i = 0; i < SoloGame.MaxFailures; i++)
        {
            game.Launch(new SecurityAttack());
        }
        game.Tick(1000);
        Assert.True(game.IsLost);
        int pointsWhenLost = game.Players[0].Points;

        TickRoom(game, room, 100);

        Assert.True(room.Heat < ServerRoom.ThrottleHeat);
        Assert.Equal(pointsWhenLost, game.Players[0].Points);
        Assert.Equal(ChallengeState.Recovering, incident.State);
    }

    [Fact]
    public void While_recovering_the_ac_works_again()
    {
        var (game, room, _) = StartCoolingFailure();
        TickRoom(game, room, 40);
        double hot = room.Heat;
        game.Repair(game.Players[0], PlayerAction.RestartCooling);

        TickRoom(game, room, 10);

        Assert.True(room.Heat < hot);
        Assert.False(game.IsRunning<OverheatingServer>());
    }
}
