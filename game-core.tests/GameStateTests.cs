using System;

namespace DuskTillDawn.Core.Tests;

public class GameStateTests
{
    private const double RealSecondsPerNight = 600;
    private const double RealSecondsPerDay = 1200;

    [Fact]
    public void A_new_night_starts_at_20_00_with_full_sla()
    {
        var state = new GameState();

        Assert.Equal("20:00", state.ClockText);
        Assert.Equal(100, state.Sla);
        Assert.Equal(NightStatus.Running, state.Status);
    }

    [Fact]
    public void One_in_game_hour_lasts_50_real_seconds()
    {
        var state = new GameState();

        state.Tick(50);

        Assert.Equal("21:00", state.ClockText);
    }

    [Fact]
    public void The_clock_passes_midnight()
    {
        var state = new GameState();

        state.Tick(50 * 4.5);

        Assert.Equal("00:30", state.ClockText);
    }

    [Fact]
    public void The_night_is_still_running_just_before_08_00()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight - 1);

        Assert.Equal(NightStatus.Running, state.Status);
    }

    [Fact]
    public void Reaching_08_00_with_sla_left_wins_after_10_real_minutes()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight);

        Assert.Equal("08:00", state.ClockText);
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
        state.Tick(50);
        state.DrainSla(100);

        state.Tick(RealSecondsPerNight);

        Assert.Equal("21:00", state.ClockText);
        Assert.Equal(NightStatus.Lost, state.Status);
    }

    [Fact]
    public void The_day_is_not_survived_just_before_the_next_20_00()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerDay - 1);

        Assert.Equal(NightStatus.Won, state.Status);
        Assert.False(state.DaySurvived);
    }

    [Fact]
    public void Reaching_the_next_20_00_after_a_won_night_survives_the_day()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerDay);

        Assert.True(state.DaySurvived);
        Assert.Equal("20:00", state.ClockText);
    }

    [Fact]
    public void The_clock_stops_once_the_day_is_survived()
    {
        var state = new GameState();
        state.Tick(RealSecondsPerDay);

        state.Tick(50);

        Assert.Equal("20:00", state.ClockText);
        Assert.Equal(20, state.HourOfDay, precision: 6);
    }

    [Fact]
    public void A_lost_night_never_survives_the_day()
    {
        var state = new GameState();
        state.DrainSla(100);

        state.Tick(RealSecondsPerDay);

        Assert.False(state.DaySurvived);
    }

    [Fact]
    public void Real_seconds_since_the_night_ended_start_at_08_00()
    {
        var state = new GameState();

        state.Tick(RealSecondsPerNight - 1);
        Assert.Equal(0, state.RealSecondsSinceNightEnded);

        state.Tick(6);
        Assert.Equal(5, state.RealSecondsSinceNightEnded, precision: 6);
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

    [Fact]
    public void The_world_clock_starts_at_20_00()
    {
        var state = new GameState();

        Assert.Equal(20, state.HourOfDay, precision: 6);
    }

    [Fact]
    public void The_world_clock_moves_in_fractions_of_an_hour()
    {
        var state = new GameState();

        state.Tick(25);

        Assert.Equal(20.5, state.HourOfDay, precision: 6);
    }

    [Fact]
    public void The_world_clock_wraps_past_midnight()
    {
        var state = new GameState();

        state.Tick(50 * 5);

        Assert.Equal(1, state.HourOfDay, precision: 6);
    }

    [Fact]
    public void The_world_clock_keeps_running_after_the_night_is_won()
    {
        var state = new GameState();
        state.Tick(RealSecondsPerNight);

        state.Tick(50);

        Assert.Equal(NightStatus.Won, state.Status);
        Assert.Equal(9, state.HourOfDay, precision: 6);
    }

    [Fact]
    public void A_full_day_takes_20_real_minutes()
    {
        var state = new GameState();

        state.Tick(20 * 60);

        Assert.Equal(20, state.HourOfDay, precision: 6);
    }
}
