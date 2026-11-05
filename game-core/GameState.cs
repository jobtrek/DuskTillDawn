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
    private const double RealSecondsPerNight = RealSecondsPerInGameHour * 8;
    private static readonly TimeOnly NightStart = new(22, 0);

    private double _elapsedRealSeconds;
    private bool _isPlayerNextToComputer;
    private bool _isComputerOpen;

    public double Sla { get; private set; } = 100;
    public NightStatus Status { get; private set; } = NightStatus.Running;

    public bool IsComputerOpen => IsRunning && _isComputerOpen;
    public bool CanOpenComputer => IsRunning && _isPlayerNextToComputer && !_isComputerOpen;
    public bool CanPlayerMove => IsRunning && !_isComputerOpen;
    public string ClockText =>
        NightStart.AddHours(_elapsedRealSeconds / RealSecondsPerInGameHour).ToString(@"HH\:mm");

    private bool IsRunning => Status == NightStatus.Running;

    public void SetPlayerNextToComputer(bool isNextToComputer) =>
        _isPlayerNextToComputer = isNextToComputer;

    public void ToggleComputer() => _isComputerOpen = CanOpenComputer;

    public void CloseComputer() => _isComputerOpen = false;

    public void Tick(double realSeconds)
    {
        if (!IsRunning)
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

        if (!IsRunning)
        {
            return;
        }

        Sla = Math.Max(Sla - percent, 0);

        if (Sla == 0)
        {
            Status = NightStatus.Lost;
        }
    }
}
