using System;
using System.Collections.Generic;
using DuskTillDawn.Core.Weather;

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

    [Fact]
    public void Skipping_goes_to_the_next_weather_and_its_rain()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);

        weather.SkipToNext();

        Assert.Equal(WeatherKind.Cloudy, weather.Current);
        Assert.Equal(WeatherProfile.For(WeatherKind.Cloudy).Rain, weather.RainIntensity);
    }

    [Fact]
    public void Skipping_after_storm_goes_back_to_clear()
    {
        var weather = new WeatherSystem(WeatherKind.Storm, Seed);

        weather.SkipToNext();

        Assert.Equal(WeatherKind.Clear, weather.Current);
        Assert.Equal(WeatherProfile.For(WeatherKind.Clear).Rain, weather.RainIntensity);
    }

    [Fact]
    public void Skipping_gives_the_new_weather_its_full_time()
    {
        var weather = new WeatherSystem(WeatherKind.Rainy, Seed);
        weather.Tick(RealSecondsPerWeather - 1);

        weather.SkipToNext();
        weather.Tick(RealSecondsPerWeather - 1);

        Assert.Equal(WeatherKind.Storm, weather.Current);
    }

    [Theory]
    [InlineData(WeatherKind.Clear)]
    [InlineData(WeatherKind.Cloudy)]
    [InlineData(WeatherKind.Rainy)]
    public void Lightning_never_strikes_outside_a_storm(WeatherKind kind)
    {
        var weather = new WeatherSystem(kind, Seed);
        int strikes = 0;
        weather.LightningStruck += () => strikes++;

        TickInSteps(weather, RealSecondsPerWeather - 1, step: 0.5);

        Assert.Equal(0, strikes);
    }

    [Fact]
    public void Lightning_strikes_now_and_then_in_a_storm()
    {
        var weather = new WeatherSystem(WeatherKind.Storm, Seed);
        int strikes = 0;
        weather.LightningStruck += () => strikes++;

        TickInSteps(weather, RealSecondsPerWeather - 1, step: 0.5);

        Assert.InRange(strikes, 2, 30);
    }

    [Fact]
    public void The_same_seed_gives_the_same_strikes()
    {
        var first = new WeatherSystem(WeatherKind.Storm, Seed);
        var second = new WeatherSystem(WeatherKind.Storm, Seed);
        var firstTimes = new List<int>();
        var secondTimes = new List<int>();
        int tick = 0;
        first.LightningStruck += () => firstTimes.Add(tick);
        second.LightningStruck += () => secondTimes.Add(tick);

        for (tick = 0; tick < 200; tick++)
        {
            first.Tick(0.5);
            second.Tick(0.5);
        }

        Assert.Equal(firstTimes, secondTimes);
    }

    [Fact]
    public void Lightning_strikes_about_as_often_at_any_frame_rate()
    {
        int slowFrames = CountStrikesOverManyStorms(step: 1.0 / 20);
        int fastFrames = CountStrikesOverManyStorms(step: 1.0 / 240);

        Assert.InRange(fastFrames, slowFrames * 0.8, slowFrames * 1.2);
    }

    [Fact]
    public void A_forced_strike_strikes_once_even_in_clear_weather()
    {
        var weather = new WeatherSystem(WeatherKind.Clear, Seed);
        int strikes = 0;
        weather.LightningStruck += () => strikes++;

        weather.ForceStrike();

        Assert.Equal(1, strikes);
    }

    private static int CountStrikesOverManyStorms(double step)
    {
        int strikes = 0;
        for (int seed = 0; seed < 50; seed++)
        {
            var weather = new WeatherSystem(WeatherKind.Storm, seed);
            weather.LightningStruck += () => strikes++;
            TickInSteps(weather, RealSecondsPerWeather - 1, step);
        }

        return strikes;
    }

    private static void TickInSteps(WeatherSystem weather, double seconds, double step)
    {
        for (double elapsed = 0; elapsed + step <= seconds; elapsed += step)
        {
            weather.Tick(step);
        }
    }
}
