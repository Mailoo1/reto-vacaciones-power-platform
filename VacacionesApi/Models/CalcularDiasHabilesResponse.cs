using System.Text.Json.Serialization;

namespace VacacionesApi.Models;

public sealed class CalcularDiasHabilesResponse
{
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
