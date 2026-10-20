// =====================================================================================
// BusinessDaysCalculator.cs - LA LÓGICA DE NEGOCIO (las reglas del reto)
// -------------------------------------------------------------------------------------
// Reglas:
//   - Lunes a sábado = día hábil.
//   - Domingo = NO hábil.
//   - Festivo = NO hábil.
//   - Se cuentan el día de inicio y el de fin (rango inclusivo).
// No sabe nada de HTTP ni de Azure: solo recibe fechas y devuelve un número. Por eso es
// tan fácil de probar con pruebas unitarias.
// =====================================================================================
namespace VacacionesApi.Services;

public sealed class BusinessDaysCalculator : IBusinessDaysCalculator
{
    public int Calculate(DateOnly start, DateOnly end, IEnumerable<DateOnly>? holidays = null)
    {
        // Defensa extra: aunque la función HTTP ya valida esto, el cálculo también se protege.
        if (end < start)
        {
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        // Se guardan los festivos en un HashSet porque preguntar "¿esta fecha es festivo?"
        // es casi instantáneo, aunque haya muchos. Si no hay festivos, queda vacío.
        var holidaySet = holidays is null ? new HashSet<DateOnly>() : new HashSet<DateOnly>(holidays);
        var businessDays = 0;

        // Se recorre día por día, desde el inicio hasta el fin (incluido).
        for (var day = start; day <= end; day = day.AddDays(1))
        {
            var isSunday = day.DayOfWeek == DayOfWeek.Sunday;

            // Solo suma si NO es domingo y NO es festivo.
            // Si un festivo cae en domingo no se resta dos veces: simplemente no se cuenta.
            if (!isSunday && !holidaySet.Contains(day))
            {
                businessDays++;
            }
        }

        return businessDays;
    }
}
