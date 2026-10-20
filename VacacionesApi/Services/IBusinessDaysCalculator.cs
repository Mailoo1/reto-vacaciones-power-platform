// =====================================================================================
// IBusinessDaysCalculator.cs - EL "CONTRATO" DEL CÁLCULO
// -------------------------------------------------------------------------------------
// Una interfaz dice QUÉ se puede hacer, sin decir CÓMO. La función HTTP solo conoce este
// contrato; la implementación real está en BusinessDaysCalculator. Esto permite cambiar o
// probar el cálculo sin tocar el endpoint.
// =====================================================================================
namespace VacacionesApi.Services;

public interface IBusinessDaysCalculator
{
    /// <summary>
    /// Cuenta los días hábiles entre dos fechas, ambas inclusive.
    /// Lunes a sábado son hábiles; domingos y festivos no.
    /// </summary>
    int Calculate(DateOnly start, DateOnly end, IEnumerable<DateOnly>? holidays = null);
}
