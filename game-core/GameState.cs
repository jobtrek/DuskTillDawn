using System;

namespace DuskTillDawn.Core;

public enum NightStatus
{
    Running,
    Won,
    Lost,
}

/// <summary>All game state lives here. Scene nodes display it, they never own it.</summary>
public sealed class GameState
{
    private const double RealSecondsPerInGameHour = 50;
    private const int NightLengthInGameHours = 12;
    private const int DayLengthInGameHours = 24;
    private const int NightStartInMinutes = 20 * 60;
    private const int MinutesPerDay = 24 * 60;
    private const double RealSecondsPerNight = RealSecondsPerInGameHour * NightLengthInGameHours;
    private const double RealSecondsPerDay = RealSecondsPerInGameHour * DayLengthInGameHours;

    private double _elapsedRealSeconds;

    public double Sla { get; private set; } = 100;
    public double Heat { get; private set; } = 20;

    public double Traffic { get; private set;} = 40;
    public double Disk {get; private set; } = 0;
    public double Power {get; private set; } = 100f;


    public int Points { get; private set; } = 1000;

    /// <summary>The night (20:00 to 08:00): Running, then Won at 08:00 or Lost when the SLA hits 0.</summary>
    public NightStatus Status { get; private set; } = NightStatus.Running;

    /// <summary>True once the clock is back at 20:00 after a won night. The game is over for this day.</summary>
    public bool DaySurvived { get; private set; }

    /// <summary>Real seconds since 08:00 (0 while the night is still running).</summary>
    public double RealSecondsSinceNightEnded => Math.Max(_elapsedRealSeconds - RealSecondsPerNight, 0);

    /// <summary>Hour of the day from 0 (inclusive) to 24 (exclusive), with fractions.</summary>
    public double HourOfDay =>
        (NightStartInMinutes / 60.0 + _elapsedRealSeconds / RealSecondsPerInGameHour) % DayLengthInGameHours;

    public string ClockText
    {
        get
        {
            int elapsedMinutes = (int)(_elapsedRealSeconds * 60 / RealSecondsPerInGameHour);
            int minuteOfDay = (NightStartInMinutes + elapsedMinutes) % MinutesPerDay;
            return $"{minuteOfDay / 60:00}:{minuteOfDay % 60:00}";
        }
    }

    public void Tick(double realSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(realSeconds);

        if (Status == NightStatus.Lost || DaySurvived)
        {
            return;
        }

        _elapsedRealSeconds = Math.Min(_elapsedRealSeconds + realSeconds, RealSecondsPerDay);

        if (_elapsedRealSeconds >= RealSecondsPerNight)
        {
            Status = NightStatus.Won;
        }

        if (_elapsedRealSeconds >= RealSecondsPerDay)
        {
            DaySurvived = true;
        }
    }

    public void LosePoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(points);
        Points -= points;
    }

    public void DrainSla(double percent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(percent);

        if (Status != NightStatus.Running)
        {
            return;
        }

        Sla = Math.Max(Sla - percent, 0);

        if (Sla <= 0)
        {
            Status = NightStatus.Lost;
        }
    }
}
