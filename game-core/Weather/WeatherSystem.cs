using System;

namespace DuskTillDawn.Core.Weather;

public sealed class WeatherSystem(WeatherKind initialWeather, int seed)
{
    private const double RealSecondsPerWeather = 120;
    private const float RainBlendRate = 0.05f;
    private const double StrikesPerSecond = 1.0 / 15;
    private double _secondsUntilChange = RealSecondsPerWeather;
    private readonly Random _random = new(seed);

    public WeatherKind Current { get; private set; } = initialWeather;

    public float RainIntensity { get; private set; } = WeatherProfile.For(initialWeather).Rain;

    public event Action? LightningStruck;

    public void Tick(double realSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(realSeconds);

        _secondsUntilChange -= realSeconds;

        if (_secondsUntilChange <= 0)
        {
            Current = NextAfter(Current);
            _secondsUntilChange = RealSecondsPerWeather;
        }

        if (Current == WeatherKind.Storm)
        {
            double chance = 1 - Math.Exp(-StrikesPerSecond * realSeconds);
            if (_random.NextDouble() < chance)
            {
                ForceStrike();
            }
        }

        float targetRain = WeatherProfile.For(Current).Rain;
        float piece = 1f - MathF.Exp(-(RainBlendRate * (float)realSeconds));
        RainIntensity = RainIntensity + (targetRain - RainIntensity) * piece;
    }

    // For testing the weather by hand: jumps straight to the next kind, with its full rain, instead of waiting.
    public void SkipToNext()
    {
        Current = (WeatherKind)(((int)Current + 1) % Enum.GetValues<WeatherKind>().Length);
        RainIntensity = WeatherProfile.For(Current).Rain;
        _secondsUntilChange = RealSecondsPerWeather;
    }

    public void ForceStrike()
    {
        LightningStruck?.Invoke();
    }

    private WeatherKind NextAfter(WeatherKind kind)
    {
        var options = OptionsFor(kind);
        int total = 0;
        foreach (var (_, weight) in options)
        {
            total += weight;
        }

        int roll = _random.Next(total);
        foreach (var (next, weight) in options)
        {
            if (roll < weight)
            {
                return next;
            }

            roll -= weight;
        }

        throw new InvalidOperationException("Should never happen");
    }

    private static (WeatherKind Next, int Weight)[] OptionsFor(WeatherKind kind)
    {
        return kind switch
        {
            WeatherKind.Clear => new[] { (WeatherKind.Clear, 50), (WeatherKind.Cloudy, 50) },
            WeatherKind.Cloudy => new[] { (WeatherKind.Clear, 40), (WeatherKind.Cloudy, 20), (WeatherKind.Rainy, 40) },
            WeatherKind.Rainy => new[] { (WeatherKind.Cloudy, 50), (WeatherKind.Rainy, 30), (WeatherKind.Storm, 20) },
            WeatherKind.Storm => new[] { (WeatherKind.Cloudy, 30), (WeatherKind.Rainy, 70) },
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
}
