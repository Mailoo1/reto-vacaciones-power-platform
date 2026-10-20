// =====================================================================================
// HolidayParser.cs - CONVIERTE LOS FESTIVOS CONFIGURADOS EN FECHAS
// -------------------------------------------------------------------------------------
// En Azure, la variable "Holidays" guarda los festivos como UN SOLO TEXTO separado por
// comas, por ejemplo: "2026-11-02,2026-11-16,2026-12-08,2026-12-25".
// Esta clase lo convierte en una lista de fechas que el cálculo puede usar.
// =====================================================================================
using System.Globalization;

namespace VacacionesApi.Services;

public static class HolidayParser
{
    /// <summary>Convierte "2026-11-02,2026-12-25" en una lista de fechas. Vacío o nulo devuelve lista vacía.</summary>
    public static IReadOnlyList<DateOnly> Parse(string? value)
    {
        // Si la variable no existe o está vacía, no hay festivos configurados: lista vacía.
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<DateOnly>();
        }

        return value
            // Parte el texto en cada coma, ignora espacios y elementos vacíos.
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            // Cada pedazo debe tener exactamente el formato yyyy-MM-dd. Si alguno está mal
            // escrito, lanza un error: preferimos enterarnos de una mala configuración a
            // ignorar un festivo en silencio y calcular mal los días.
            .Select(text => DateOnly.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture))
            .ToList();
    }
}