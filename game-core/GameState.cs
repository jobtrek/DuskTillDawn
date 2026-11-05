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
    private const double RealSecondsPerInGameHour = 75;
    private const int NightLengthInGameHours = 8;
    private const int NightStartInMinutes = 22 * 60;
    private const int MinutesPerDay = 24 * 60;
    private const double RealSecondsPerNight = RealSecondsPerInGameHour * NightLengthInGameHours;

    private double _elapsedRealSeconds;

    public double Sla { get; private set; } = 100;

    public NightStatus Status { get; private set; } = NightStatus.Running;

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

        if (Status != NightStatus.Running)
        {
            return;
        }

        _elapsedRealSeconds = Math.Min(_elapsedRealSeconds + realSeconds, RealSecondsPerNight);

        if (_elapsedRealSeconds >= RealSecondsPerNight)
        {
            Status = NightStatus.Won;
        }
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
