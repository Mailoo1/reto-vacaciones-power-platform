using System.Text.Json.Serialization;

namespace VacacionesApi.Models;

public sealed class CalcularDiasHabilesRequest
{
    [JsonPropertyName("fechaInicio")]
    public DateOnly? FechaInicio { get; init; }

    [JsonPropertyName("fechaFin")]
    public DateOnly? FechaFin { get; init; }

    /// <summary>Opcional. Si no se envía, se usan los festivos configurados en la app setting "Holidays".</summary>
    [JsonPropertyName("festivos")]
    public List<DateOnly>? Festivos { get; init; }
}
