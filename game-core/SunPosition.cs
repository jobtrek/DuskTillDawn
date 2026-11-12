using System;

namespace DuskTillDawn.Core;

public static class SunPosition
{
    private const double SunriseHour = 8;
    private const double SunsetHour = 20;
    private const double DaylightHours = SunsetHour - SunriseHour;
    private const double MaxElevationDegrees = 60;
    private const double EastAzimuthDegrees = -90;
    private const double WestAzimuthDegrees = 90;

    public static (double AzimuthDegrees, double ElevationDegrees, bool IsUp) At(double hourOfDay)
    {
        if (hourOfDay < SunriseHour || hourOfDay >= SunsetHour)
        {
            return (WestAzimuthDegrees, 0, false);
        }

        double t = (hourOfDay - SunriseHour) / DaylightHours;
        double elevation = MaxElevationDegrees * Math.Sin(Math.PI * t);
        double azimuth = EastAzimuthDegrees + (WestAzimuthDegrees - EastAzimuthDegrees) * t;

        return (azimuth, elevation, true);
    }
}
