// =====================================================================================
// CalcularDiasHabilesRequest.cs - FORMA DEL JSON QUE LLEGA A LA API
// -------------------------------------------------------------------------------------
// Ejemplo de lo que envía Power Apps:
//   { "fechaInicio": "2026-11-02", "fechaFin": "2026-11-07", "festivos": ["2026-11-02"] }
// =====================================================================================
using System.Text.Json.Serialization;

namespace VacacionesApi.Models;

public sealed class CalcularDiasHabilesRequest
{
    // DateOnly = solo fecha, sin hora (evita problemas de zonas horarias).
    // El signo "?" (nullable) permite saber si el campo NO vino, para poder avisar que es obligatorio.
    // [JsonPropertyName] fija el nombre exacto que se espera en el JSON.
    [JsonPropertyName("fechaInicio")]
    public DateOnly? FechaInicio { get; init; }

    [JsonPropertyName("fechaFin")]
    public DateOnly? FechaFin { get; init; }

    /// <summary>Opcional. Si no se envía, se usan los festivos configurados en la variable "Holidays".</summary>
    [JsonPropertyName("festivos")]
    public List<DateOnly>? Festivos { get; init; }
}

