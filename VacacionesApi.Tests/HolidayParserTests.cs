// =====================================================================================
// HolidayParserTests.cs - PRUEBAS DE LA LECTURA DE FESTIVOS CONFIGURADOS
// -------------------------------------------------------------------------------------
// Comprueban que el texto de la variable "Holidays" se convierte bien en fechas.
// =====================================================================================
using VacacionesApi.Services;
using Xunit;

namespace VacacionesApi.Tests;

public class HolidayParserTests
{
    // Un [Theory] repite la misma prueba con varios valores. Aquí: nulo, vacío y solo espacios
    // deben dar una lista vacía (3 pruebas en una).
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyValue_ReturnsEmptyList(string? value)
    {
        Assert.Empty(HolidayParser.Parse(value));
    }

    // Dos fechas separadas por coma (con un espacio de más) deben convertirse en dos fechas.
    [Fact]
    public void CommaSeparatedValue_ReturnsDates()
    {
        var result = HolidayParser.Parse("2026-11-02, 2026-12-25");
        Assert.Equal(new[] { new DateOnly(2026, 11, 2), new DateOnly(2026, 12, 25) }, result);
    }
}