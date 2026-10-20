using VacacionesApi.Services;
using Xunit;

namespace VacacionesApi.Tests;

public class HolidayParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyValue_ReturnsEmptyList(string? value)
    {
        Assert.Empty(HolidayParser.Parse(value));
    }

    [Fact]
    public void CommaSeparatedValue_ReturnsDates()
    {
        var result = HolidayParser.Parse("2026-11-02, 2026-12-25");
        Assert.Equal(new[] { new DateOnly(2026, 11, 2), new DateOnly(2026, 12, 25) }, result);
    }
}
