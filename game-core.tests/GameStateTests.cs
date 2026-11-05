using System;

namespace DuskTillDawn.Core.Tests;

public class GameStateTests
{
    private const double RealSecondsPerNight = 600;

    private static GameState StateWithPlayerAtComputer()
    {
        var state = new GameState();
        state.SetPlayerNextToComputer(true);
        return state;
    }

    private static GameState StateWithOpenComputer()
    {
        GameState state = StateWithPlayerAtComputer();
        state.ToggleComputer();
        return state;
    }

    [Theory]
    [InlineData(0, "22:00")]
    [InlineData(75, "23:00")]
    [InlineData(187.5, "00:30")]
    [InlineData(RealSecondsPerNight, "06:00")]
    [InlineData(RealSecondsPerNight * 2, "06:00")]
    public void The_clock_runs_from_22_00_to_06_00(double realSeconds, string expected)
    {
        var state = new GameState();

        state.Tick(realSeconds);

        Assert.Equal(expected, state.ClockText);
    }

    [Theory]
    [InlineData(RealSecondsPerNight - 1, NightStatus.Running)]
    [InlineData(RealSecondsPerNight, NightStatus.Won)]
    public void Reaching_06_00_with_sla_left_wins(double realSeconds, NightStatus expected)
    {
        var state = new GameState();

        state.Tick(realSeconds);

        Assert.Equal(expected, state.Status);
    }

    [Fact]
    public void Many_small_ticks_reach_the_end_of_the_night()
    {
        var state = new GameState();

        for (var i = 0; i < RealSecondsPerNight * 60 + 1; i++)
        {
            state.Tick(1.0 / 60.0);
        }

        Assert.Equal(NightStatus.Won, state.Status);
    }

    [Fact]
    public void Draining_the_sla_lowers_it()
    {
        var state = new GameState();

        state.DrainSla(12.5);

        Assert.Equal(87.5, state.Sla);
        Assert.Equal(NightStatus.Running, state.Status);
    }

    [Fact]
    public void Sla_reaching_zero_loses_the_night_and_never_goes_below_zero()
    {
        var state = new GameState();

        state.DrainSla(150);

        Assert.Equal(0, state.Sla);
        Assert.Equal(NightStatus.Lost, state.Status);
    }

    [Fact]
    public void Time_stops_once_the_night_is_lost()
    {
        var state = new GameState();
        state.Tick(75);
        state.DrainSla(100);

        state.Tick(RealSecondsPerNight);

        Assert.Equal("23:00", state.ClockText);
        Assert.Equal(NightStatus.Lost, state.Status);
    }

    [Fact]
    public void Sla_cannot_change_after_the_night_is_won()
    {
        var state = new GameState();
        state.Tick(RealSecondsPerNight);

        state.DrainSla(100);

        Assert.Equal(100, state.Sla);
        Assert.Equal(NightStatus.Won, state.Status);
    }

    [Fact]
    public void The_sla_cannot_be_raised_by_a_negative_drain()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameState().DrainSla(-1));
    }

    [Fact]
    public void The_computer_cannot_be_opened_from_far_away()
    {
        var state = new GameState();
        Assert.False(state.CanOpenComputer);

        state.ToggleComputer();

        Assert.False(state.IsComputerOpen);
    }

    [Fact]
    public void Toggling_next_to_the_computer_opens_then_closes_it()
    {
        GameState state = StateWithPlayerAtComputer();
        Assert.True(state.CanOpenComputer);

        state.ToggleComputer();
        Assert.True(state.IsComputerOpen);
        Assert.False(state.CanOpenComputer);

        state.ToggleComputer();
        Assert.False(state.IsComputerOpen);
    }

    [Fact]
    public void The_player_can_move_only_while_the_computer_is_closed()
    {
        GameState state = StateWithOpenComputer();
        Assert.False(state.CanPlayerMove);

        state.CloseComputer();

        Assert.False(state.IsComputerOpen);
        Assert.True(state.CanPlayerMove);
    }

    [Fact]
    public void The_clock_keeps_running_while_the_computer_is_open()
    {
        GameState state = StateWithOpenComputer();

        state.Tick(75);

        Assert.Equal("23:00", state.ClockText);
    }

    [Fact]
    public void The_computer_closes_when_the_night_is_won()
    {
        GameState state = StateWithOpenComputer();

        state.Tick(RealSecondsPerNight);

        Assert.False(state.IsComputerOpen);
    }

    [Fact]
    public void The_computer_closes_when_the_night_is_lost()
    {
        GameState state = StateWithOpenComputer();

        state.DrainSla(100);

        Assert.False(state.IsComputerOpen);
    }

    [Fact]
    public void Nothing_can_be_used_after_the_night_ended()
    {
        GameState state = StateWithPlayerAtComputer();
        state.DrainSla(100);

        state.ToggleComputer();

        Assert.False(state.CanOpenComputer);
        Assert.False(state.IsComputerOpen);
        Assert.False(state.CanPlayerMove);
    }
}
