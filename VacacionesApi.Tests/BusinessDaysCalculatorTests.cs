// =====================================================================================
// BusinessDaysCalculatorTests.cs - PRUEBAS UNITARIAS DEL CÁLCULO
// -------------------------------------------------------------------------------------
// Cada prueba es un caso concreto con su resultado esperado. Si alguien cambia la lógica
// y rompe una regla, la prueba falla y avisa. Se ejecutan con:  dotnet test VacacionesApi.Tests
// Nota sobre las fechas: el lunes 2 de noviembre de 2026 es el día de referencia.
// =====================================================================================
using VacacionesApi.Services;
using Xunit;

namespace VacacionesApi.Tests;

public class BusinessDaysCalculatorTests
{
    private readonly BusinessDaysCalculator _calculator = new();

    // Atajo para escribir fechas en texto: D("2026-11-02").
    private static DateOnly D(string iso) => DateOnly.ParseExact(iso, "yyyy-MM-dd");

    // Es el ejemplo exacto del documento del reto: lunes 2 a sábado 7 con el 2 festivo = 5.
    [Fact]
    public void ExampleFromChallenge_ReturnsFive()
    {
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-07"), new[] { D("2026-11-02") });
        Assert.Equal(5, result);
    }

    // Una semana completa de lunes a domingo, sin festivos: 6 (el domingo no cuenta).
    [Fact]
    public void FullWeekWithoutHolidays_ExcludesOnlySunday()
    {
        // lunes 2 a domingo 8
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-08"));
        Assert.Equal(6, result);
    }

    // Un rango de un solo día que cae en domingo: 0 días hábiles.
    [Fact]
    public void SingleSunday_ReturnsZero()
    {
        Assert.Equal(0, _calculator.Calculate(D("2026-11-08"), D("2026-11-08")));
    }

    // Un rango de un solo día lunes: 1 día hábil (el rango es inclusivo).
    [Fact]
    public void SingleMonday_ReturnsOne()
    {
        Assert.Equal(1, _calculator.Calculate(D("2026-11-02"), D("2026-11-02")));
    }

    // Un festivo que cae en domingo no debe restar de más: el domingo ya no contaba.
    [Fact]
    public void HolidayOnSunday_DoesNotSubtractTwice()
    {
        var result = _calculator.Calculate(D("2026-11-02"), D("2026-11-08"), new[] { D("2026-11-08") });
        Assert.Equal(6, result);
    }

    // Un rango que cruza de un mes a otro, con y sin festivo.
    [Fact]
    public void RangeCrossingMonths_CountsCorrectly()
    {
        // viernes 30 oct a martes 3 nov: vie, sáb, (dom), lun, mar
        Assert.Equal(4, _calculator.Calculate(D("2026-10-30"), D("2026-11-03")));
        Assert.Equal(3, _calculator.Calculate(D("2026-10-30"), D("2026-11-03"), new[] { D("2026-11-02") }));
    }

    // Si la fecha final es anterior a la inicial, el cálculo debe fallar con un error claro.
    [Fact]
    public void EndBeforeStart_Throws()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Calculate(D("2026-11-07"), D("2026-11-02")));
    }
}
