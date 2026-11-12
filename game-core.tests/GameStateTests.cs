using System;

namespace DuskTillDawn.Core.Tests;

public class GameStateTests
{
    private const double RealSecondsPerNight = 600;

    [Fact]
    public void A_new_night_starts_at_22_00_with_full_sla()
    {
        var state = new GameState();

        Assert.Equal("22:00", state.ClockText);
        Assert.Equal(100, state.Sla);
        Assert.Equal(NightStatus.Running, state.Status);
    }

    [Fact]
    public void One_in_game_hour_lasts_75_real_seconds()
    {
        var state = new GameState();

        state.Tick(75);

        Assert.Equal("23:00", state.ClockText);
    }

    [Fact]
    public void The_clock_passes_midnight()
    {
        var state = new GameState();

        state.Tick(75 * 2.5);

        Assert.Equal("00:30", state.ClockText);
    }

    [Fact]
    public void The_night_is_still_running_just_before_06_00()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight - 1);

        Assert.Equal(NightStatus.Running, state.Status);
    }

    [Fact]
    public void Reaching_06_00_with_sla_left_wins_after_10_real_minutes()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight);

        Assert.Equal("06:00", state.ClockText);
        Assert.Equal(NightStatus.Won, state.Status);
    }

    [Fact]
    public void Many_small_ticks_reach_the_same_end_as_one_big_tick()
    {
        var state = new GameState();

        for (var i = 0; i < 60 * 600; i++)
        {
            state.Tick(1.0 / 60.0);
        }
        state.Tick(0.001);

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
    public void The_clock_never_goes_past_06_00()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight * 2);

        Assert.Equal("06:00", state.ClockText);
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
    public void Time_cannot_go_backwards()
    {
        var state = new GameState();

        Assert.Throws<ArgumentOutOfRangeException>(() => state.Tick(-1));
    }

    [Fact]
    public void The_sla_cannot_be_raised_by_a_negative_drain()
    {
        var state = new GameState();

        Assert.Throws<ArgumentOutOfRangeException>(() => state.DrainSla(-1));
    }
}
