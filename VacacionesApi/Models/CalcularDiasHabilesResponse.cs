// =====================================================================================
// CalcularDiasHabilesResponse.cs - FORMA DEL JSON QUE DEVUELVE LA API
// -------------------------------------------------------------------------------------
// Ejemplo: { "fechaInicio": "2026-11-02", "fechaFin": "2026-11-07", "diasHabiles": 5 }
// Power Apps toma el campo "diasHabiles" y lo muestra en pantalla.
// =====================================================================================
using System.Text.Json.Serialization;

namespace VacacionesApi.Models;

public sealed class CalcularDiasHabilesResponse
{
    // Constructor: la respuesta se crea con los tres datos y ya no cambia (solo lectura).
    public CalcularDiasHabilesResponse(DateOnly fechaInicio, DateOnly fechaFin, int diasHabiles)
    {
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        DiasHabiles = diasHabiles;
    }

    [JsonPropertyName("fechaInicio")]
    public DateOnly FechaInicio { get; }

    [JsonPropertyName("fechaFin")]
    public DateOnly FechaFin { get; }

    [JsonPropertyName("diasHabiles")]
    public int DiasHabiles { get; }
}
