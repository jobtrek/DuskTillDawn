namespace DuskTillDawn.Core.Tests;

public class SunPositionTests
{
    private const double MaxElevationDegrees = 60;
    private const double EastAzimuthDegrees = -90;
    private const double WestAzimuthDegrees = 90;

    [Fact]
    public void At_08_00_the_sun_rises_in_the_east()
    {
        var sun = SunPosition.At(8);

        Assert.Equal(0, sun.ElevationDegrees, precision: 6);
        Assert.Equal(EastAzimuthDegrees, sun.AzimuthDegrees, precision: 6);
    }

    [Fact]
    public void At_14_00_the_sun_is_at_its_highest()
    {
        var sun = SunPosition.At(14);

        Assert.Equal(MaxElevationDegrees, sun.ElevationDegrees, precision: 6);
    }

    [Fact]
    public void At_20_00_the_sun_sets_in_the_west()
    {
        var sun = SunPosition.At(20);

        Assert.Equal(0, sun.ElevationDegrees, precision: 6);
        Assert.Equal(WestAzimuthDegrees, sun.AzimuthDegrees, precision: 6);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(22)]
    public void The_sun_is_down_at_night(double hourOfDay)
    {
        var sun = SunPosition.At(hourOfDay);

        Assert.False(sun.IsUp);
    }

    [Fact]
    public void The_sun_rises_until_14_00_then_falls_until_20_00()
    {
        for (var hour = 8; hour < 14; hour++)
        {
            Assert.True(SunPosition.At(hour + 1).ElevationDegrees > SunPosition.At(hour).ElevationDegrees);
        }

        for (var hour = 14; hour < 20; hour++)
        {
            Assert.True(SunPosition.At(hour + 1).ElevationDegrees < SunPosition.At(hour).ElevationDegrees);
        }
    }
}
