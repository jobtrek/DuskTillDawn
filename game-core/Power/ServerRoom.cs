using System;
using DuskTillDawn.Core.Incidents;

namespace DuskTillDawn.Core.Power;

// The basement: room temperature at the server intakes, the battery the solar panels fill, and whether
// the servers run. Heat and battery follow the game clock; the UPS and reboots count real seconds,
// because the player has to react to them.
public sealed class ServerRoom
{
    public const double MinHeat = 18;
    public const double MaxHeat = 55;
    public const double WarningHeat = 27;
    public const double ThrottleHeat = 32;
    public const double ShutdownHeat = 45;

    public const double NormalTraffic = 40;
    public const double DdosTraffic = 130;
    public const double ReducedCapacity = 70;

    public const double LowBattery = 15;
    public const double UpsSeconds = 15;
    public const double CleanRebootSeconds = 10;
    public const double CrashRebootSeconds = 30;

    private const double RealSecondsPerInGameMinute = 50.0 / 60;

    private const double HeatChangePerMinute = 0.3;
    private const double CooledHeat = 22;
    private const double HeatAtFullLoad = 6;
    private const double HeatPerExtraStage = 2;
    private const double ServersOffHeat = 24;

    private const double IdleDrainPerMinute = 0.08;
    private const double LoadDrainPerMinute = 0.1;
    private const double ExtraStageDrainShare = 0.25;
    private const double WarningDrawFactor = 1.2;
    private const double EcoDrawFactor = 0.7;
    private const double BaselineDrainPerMinute = IdleDrainPerMinute + LoadDrainPerMinute * NormalTraffic / 100;

    private const double SolarStartHour = 6;
    private const double SolarEndHour = 8;
    private const double SolarChargePerMinute = 0.5;
    private const double DirtyPanelsMaxCut = 0.6;
    private const double DirtyStartCharge = 50;
    private const double PowerBackBattery = 5;

    private const double ServersDownSlaDrainPerMinute = 1;

    private double _secondsLeft;

    public double Heat { get; private set; } = CooledHeat;
    public double Battery { get; private set; } = 100;
    public double Traffic { get; private set; } = NormalTraffic;
    public int Stage { get; private set; } = 1;
    public double PanelCleanliness { get; private set; } = 1;
    public bool EcoMode { get; private set; }
    public RoomPowerState Status { get; private set; } = RoomPowerState.Running;

    // Seconds left on the UPS, or before a reboot finishes.
    public double SecondsLeft => _secondsLeft;

    public bool ServersServing => Status is RoomPowerState.Running or RoomPowerState.OnUps;

    public HeatZone Zone => Heat switch
    {
        >= ShutdownHeat => HeatZone.Shutdown,
        >= ThrottleHeat => HeatZone.Throttling,
        >= WarningHeat => HeatZone.Warning,
        _ => HeatZone.Optimal,
    };

    public bool CanTurnOnEcoMode => Battery <= LowBattery && !EcoMode;

    public bool CanReboot => Status == RoomPowerState.ShutDownByHeat && Heat < ThrottleHeat && Battery > 0;

    public double SlaDrainPerMinute => ServersServing ? 0 : ServersDownSlaDrainPerMinute;

    // 1 on a normal night; above 1 the electrical equipment works harder and faults get likelier.
    public double PowerDraw => DrainPerMinute(Traffic) / BaselineDrainPerMinute;

    // Cleaner panels during the day mean a fuller battery at 20:00.
    public void StartNight(double panelCleanliness)
    {
        PanelCleanliness = Math.Clamp(panelCleanliness, 0, 1);
        Battery = DirtyStartCharge + (100 - DirtyStartCharge) * PanelCleanliness;
        Heat = CooledHeat;
        Traffic = NormalTraffic;
        EcoMode = false;
        Status = RoomPowerState.Running;
    }

    public void SetStage(int stage)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(stage, 1);
        Stage = stage;
    }

    public void Tick(double realSeconds, double hourOfDay, Game game)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(realSeconds);

        double minutes = realSeconds / RealSecondsPerInGameMinute;
        bool coolingBroken = game.IsRunning<OverheatingServer>();
        double demand = game.IsRunning<SecurityAttack>() ? DdosTraffic : NormalTraffic;

        Traffic = ServersServing ? Math.Min(demand, Capacity()) : 0;
        UpdateHeat(minutes, coolingBroken);
        UpdateBattery(minutes, hourOfDay);
        UpdateStatus(realSeconds);
    }

    public bool TryTurnOnEcoMode()
    {
        if (!CanTurnOnEcoMode)
        {
            return false;
        }

        EcoMode = true;
        return true;
    }

    public void TurnOffEcoMode()
    {
        EcoMode = false;
    }

    public bool TryGracefulShutdown()
    {
        if (Status != RoomPowerState.OnUps)
        {
            return false;
        }

        Status = RoomPowerState.ShutDownCleanly;
        return true;
    }

    public bool TryReboot()
    {
        if (!CanReboot)
        {
            return false;
        }

        StartReboot(CleanRebootSeconds);
        return true;
    }

    private double Capacity()
    {
        return EcoMode || Zone == HeatZone.Throttling ? ReducedCapacity : 100;
    }

    private void UpdateHeat(double minutes, bool coolingBroken)
    {
        double target;
        if (!ServersServing)
        {
            target = ServersOffHeat;
        }
        else if (coolingBroken)
        {
            target = MaxHeat;
        }
        else
        {
            target = CooledHeat + HeatAtFullLoad * Traffic / 100 + HeatPerExtraStage * (Stage - 1);
        }

        double step = HeatChangePerMinute * minutes;
        Heat = Math.Clamp(Heat < target ? Math.Min(Heat + step, target) : Math.Max(Heat - step, target), MinHeat, MaxHeat);
    }

    private double DrainPerMinute(double traffic)
    {
        double drain = (IdleDrainPerMinute + LoadDrainPerMinute * traffic / 100) * (1 + ExtraStageDrainShare * (Stage - 1));
        if (Zone == HeatZone.Warning)
        {
            drain *= WarningDrawFactor;
        }

        if (EcoMode)
        {
            drain *= EcoDrawFactor;
        }

        return drain;
    }

    private void UpdateBattery(double minutes, double hourOfDay)
    {
        double change = 0;
        if (Status != RoomPowerState.ShutDownCleanly && Status != RoomPowerState.Crashed && Status != RoomPowerState.ShutDownByHeat)
        {
            change -= DrainPerMinute(Traffic) * minutes;
        }

        if (hourOfDay >= SolarStartHour && hourOfDay < SolarEndHour)
        {
            double cut = DirtyPanelsMaxCut * (1 - PanelCleanliness);
            change += SolarChargePerMinute * (1 - cut) * minutes;
        }

        Battery = Math.Clamp(Battery + change, 0, 100);
    }

    private void UpdateStatus(double realSeconds)
    {
        switch (Status)
        {
            case RoomPowerState.Running:
                if (Heat >= ShutdownHeat)
                {
                    Status = RoomPowerState.ShutDownByHeat;
                }
                else if (Battery <= 0)
                {
                    Status = RoomPowerState.OnUps;
                    _secondsLeft = UpsSeconds;
                }

                break;
            case RoomPowerState.OnUps:
                _secondsLeft -= realSeconds;
                if (Heat >= ShutdownHeat)
                {
                    Status = RoomPowerState.ShutDownByHeat;
                }
                else if (Battery > 0)
                {
                    Status = RoomPowerState.Running;
                }
                else if (_secondsLeft <= 0)
                {
                    Status = RoomPowerState.Crashed;
                }

                break;
            case RoomPowerState.ShutDownCleanly:
            case RoomPowerState.Crashed:
                if (Battery >= PowerBackBattery)
                {
                    StartReboot(Status == RoomPowerState.Crashed ? CrashRebootSeconds : CleanRebootSeconds);
                }

                break;
            case RoomPowerState.Rebooting:
                _secondsLeft -= realSeconds;
                if (_secondsLeft <= 0)
                {
                    Status = RoomPowerState.Running;
                }

                break;
        }
    }

    private void StartReboot(double seconds)
    {
        Status = RoomPowerState.Rebooting;
        _secondsLeft = seconds;
    }
}
