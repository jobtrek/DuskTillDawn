using System;
using System.Collections.Generic;

namespace DuskTillDawn.Core.Tests;

public class WeatherSystemTests
{
    private const int Seed = 42;
    private const double RealSecondsPerWeather = 120;
    private const int ManyChanges = 1000;

    [Fact]
    public void It_starts_with_the_given_weather_and_its_rain()
    {
        var weather = new WeatherSystem(WeatherKind.Storm, Seed);

        Assert.Equal(WeatherKind.Storm, weather.Current);
        Assert.Equal(WeatherProfile.For(WeatherKind.Storm).Rain, weather.RainIntensity);
    }

    [Fact]
    public void The_weather_does_not_change_before_its_time_is_up()
    {
        var weather = new WeatherSystem(WeatherKind.Rainy, Seed);

        weather.Tick(RealSecondsPerWeather - 1);

        Assert.Equal(WeatherKind.Rainy, weather.Current);
    }

    [Fact]
    public void The_same_seed_gives_the_same_weather()
    {
        var first = new WeatherSystem(WeatherKind.Clear, Seed);
        var second = new WeatherSystem(WeatherKind.Clear, Seed);

        for (var i = 0; i < ManyChanges; i++)
        {
            first.Tick(RealSecondsPerWeather);
            second.Tick(RealSecondsPerWeather);

            Assert.Equal(first.Current, second.Current);
        }
    }

    [Fact]
    public void Every_kind_of_weather_happens_over_time()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);
        var seen = new HashSet<WeatherKind>();

        for (var i = 0; i < ManyChanges; i++)
        {
            weather.Tick(RealSecondsPerWeather);
            seen.Add(weather.Current);
        }

        Assert.Equal(Enum.GetValues<WeatherKind>().Length, seen.Count);
    }

    [Fact]
    public void Clear_and_storm_never_follow_each_other_directly()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);

        for (var i = 0; i < ManyChanges; i++)
        {
            WeatherKind previous = weather.Current;
            weather.Tick(RealSecondsPerWeather);

            Assert.False(previous == WeatherKind.Clear && weather.Current == WeatherKind.Storm);
            Assert.False(previous == WeatherKind.Storm && weather.Current == WeatherKind.Clear);
        }
    }

    [Fact]
    public void Rain_stays_between_0_and_1()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);

        for (var i = 0; i < ManyChanges * 10; i++)
        {
            weather.Tick(1.7);

            Assert.InRange(weather.RainIntensity, 0f, 1f);
        }
    }

    [Fact]
    public void Rain_fades_in_instead_of_snapping()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);

        // Tick one second at a time until the weather turns to one with rain.
        var guard = 0;
        while (WeatherProfile.For(weather.Current).Rain == 0f)
        {
            weather.Tick(1);
            guard++;
            Assert.True(guard < ManyChanges * RealSecondsPerWeather, "It never started raining.");
        }

        float target = WeatherProfile.For(weather.Current).Rain;
        Assert.True(weather.RainIntensity < target * 0.1f, "Rain jumped straight to its target.");

        for (var second = 0; second < 60; second++)
        {
            weather.Tick(1);
        }

        Assert.True(weather.RainIntensity > target * 0.9f, "Rain did not get close to its target after a minute.");
    }

    [Fact]
    public void Negative_time_is_rejected()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);

        Assert.Throws<ArgumentOutOfRangeException>(() => weather.Tick(-1));
    }
}
