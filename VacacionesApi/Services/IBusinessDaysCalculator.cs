namespace VacacionesApi.Services;

public interface IBusinessDaysCalculator
{
    /// <summary>
    /// Cuenta los días hábiles entre dos fechas, ambas inclusive.
    /// Lunes a sábado son hábiles; domingos y festivos no.
    /// </summary>
    int Calculate(DateOnly start, DateOnly end, IEnumerable<DateOnly>? holidays = null);
}
