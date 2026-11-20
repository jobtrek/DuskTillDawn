using System;

namespace DuskTillDawn.Core;

public enum WeatherKind
{
    Clear,
    Cloudy,
    Rainy,
    Storm,
}

public sealed record WeatherProfile(float Rain)
{
    public static WeatherProfile For(WeatherKind kind) => kind switch
    {
        WeatherKind.Clear => new WeatherProfile(0f),
        WeatherKind.Cloudy => new WeatherProfile(0f),
        WeatherKind.Rainy => new WeatherProfile(0.6f),
        WeatherKind.Storm => new WeatherProfile(1f),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };
}
