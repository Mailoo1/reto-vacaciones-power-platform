using VacacionesApi.Services;
using Xunit;

namespace VacacionesApi.Tests;

public class BusinessDaysCalculatorTests
{
    private readonly BusinessDaysCalculator _calculator = new();

    private static DateOnly D(string iso) => DateOnly.ParseExact(iso, "yyyy-MM-dd");

    [Fact]
    public void ExampleFromChallenge_ReturnsFive()
    {
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-07"), new[] { D("2026-11-02") });
        Assert.Equal(5, result);
    }

    [Fact]
    public void FullWeekWithoutHolidays_ExcludesOnlySunday()
    {
        // lunes 2 a domingo 8
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-08"));
        Assert.Equal(6, result);
    }

    [Fact]
    public void SingleSunday_ReturnsZero()
    {
        Assert.Equal(0, _calculator.Calculate(D("2026-11-08"), D("2026-11-08")));
    }

    [Fact]
    public void SingleMonday_ReturnsOne()
    {
        Assert.Equal(1, _calculator.Calculate(D("2026-11-02"), D("2026-11-02")));
    }

    [Fact]
    public void HolidayOnSunday_DoesNotSubtractTwice()
    {
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-08"), new[] { D("2026-11-08") });
        Assert.Equal(6, result);
    }

    [Fact]
    public void RangeCrossingMonths_CountsCorrectly()
    {
        // viernes 30 oct a martes 3 nov: vie, sáb, (dom), lun, mar
        Assert.Equal(4, _calculator.Calculate(D("2026-10-30"), D("2026-11-03")));
        Assert.Equal(3, _calculator.Calculate(D("2026-10-30"), D("2026-11-03"), new[] { D("2026-11-02") }));
    }

    [Fact]
    public void EndBeforeStart_Throws()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Calculate(D("2026-11-07"), D("2026-11-02")));
    }
}
