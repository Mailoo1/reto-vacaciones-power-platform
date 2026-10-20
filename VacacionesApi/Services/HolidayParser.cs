using System.Globalization;

namespace VacacionesApi.Services;

public static class HolidayParser
{
    /// <summary>Convierte "2026-11-02,2026-12-25" en una lista de fechas. Vacío o nulo devuelve lista vacía.</summary>
    public static IReadOnlyList<DateOnly> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<DateOnly>();
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(text => DateOnly.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture))
            .ToList();
    }
}
