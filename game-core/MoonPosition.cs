using System;

namespace DuskTillDawn.Core;

/// <summary>The moon takes over from the sun: it rises at 20:00, is highest at 02:00 and sets at 08:00.</summary>
public static class MoonPosition
{
    private const double MoonriseHour = 20;
    private const double MoonsetHour = 8;
    private const double HoursPerDay = 24;
    private const double NightHours = MoonsetHour + HoursPerDay - MoonriseHour;
    private const double MaxElevationDegrees = 45;
    private const double EastAzimuthDegrees = -90;
    private const double WestAzimuthDegrees = 90;

    /// <summary>Elevation is never negative: the moon is either above the horizon or not up at all.</summary>
    public static (double AzimuthDegrees, double ElevationDegrees, bool IsUp) At(double hourOfDay)
    {
        if (hourOfDay >= MoonsetHour && hourOfDay < MoonriseHour)
        {
            return (WestAzimuthDegrees, 0, false);
        }

        double hoursSinceMoonrise = (hourOfDay - MoonriseHour + HoursPerDay) % HoursPerDay;
        double t = hoursSinceMoonrise / NightHours;
        double elevation = MaxElevationDegrees * Math.Sin(Math.PI * t);
        double azimuth = EastAzimuthDegrees + (WestAzimuthDegrees - EastAzimuthDegrees) * t;

        return (azimuth, elevation, true);
    }
}
