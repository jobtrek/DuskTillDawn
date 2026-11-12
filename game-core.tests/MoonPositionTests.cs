namespace DuskTillDawn.Core.Tests;

public class MoonPositionTests
{
    private const double MaxElevationDegrees = 45;

    [Fact]
    public void At_20_00_the_moon_rises_in_the_east()
    {
        var moon = MoonPosition.At(20);

        Assert.True(moon.IsUp);
        Assert.Equal(0, moon.ElevationDegrees, precision: 6);
        Assert.Equal(-90, moon.AzimuthDegrees, precision: 6);
    }

    [Fact]
    public void At_02_00_the_moon_is_at_its_highest()
    {
        Assert.Equal(MaxElevationDegrees, MoonPosition.At(2).ElevationDegrees, precision: 6);
    }

    [Fact]
    public void At_08_00_the_moon_sets_and_the_sun_takes_over()
    {
        Assert.False(MoonPosition.At(8).IsUp);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(14)]
    [InlineData(19.9)]
    public void The_moon_is_down_during_the_day(double hourOfDay)
    {
        Assert.False(MoonPosition.At(hourOfDay).IsUp);
    }

    [Fact]
    public void The_moon_is_never_below_the_ground()
    {
        for (double hour = 0; hour < 24; hour += 0.25)
        {
            Assert.True(MoonPosition.At(hour).ElevationDegrees >= 0);
        }
    }

    [Fact]
    public void The_moon_rises_until_02_00_then_falls_until_08_00()
    {
        for (var hour = 20; hour < 26; hour++)
        {
            Assert.True(MoonPosition.At((hour + 1) % 24).ElevationDegrees > MoonPosition.At(hour % 24).ElevationDegrees);
        }

        for (var hour = 2; hour < 8; hour++)
        {
            Assert.True(MoonPosition.At(hour + 1).ElevationDegrees < MoonPosition.At(hour).ElevationDegrees);
        }
    }
}
