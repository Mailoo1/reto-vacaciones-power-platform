using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VacacionesApi.Models;
using VacacionesApi.Services;

namespace VacacionesApi.Functions;

public class CalcularDiasHabilesFunction
{
    private const int MaxRangeInDays = 366;

    private readonly IBusinessDaysCalculator _calculator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CalcularDiasHabilesFunction> _logger;

    public CalcularDiasHabilesFunction(
        IBusinessDaysCalculator calculator,
        IConfiguration configuration,
        ILogger<CalcularDiasHabilesFunction> logger)
    {
        _calculator = calculator;
        _configuration = configuration;
        _logger = logger;
    }

    [Function("CalcularDiasHabiles")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "dias-habiles")] HttpRequest req)
    {
        CalcularDiasHabilesRequest? request;
        try
        {
            request = await req.ReadFromJsonAsync<CalcularDiasHabilesRequest>();
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return Error("El cuerpo debe ser un JSON válido con fechas en formato yyyy-MM-dd.");
        }

        if (request?.FechaInicio is null || request.FechaFin is null)
        {
            return Error("Los campos 'fechaInicio' y 'fechaFin' son obligatorios.");
        }

        var start = request.FechaInicio.Value;
        var end = request.FechaFin.Value;

        if (end < start)
        {
            return Error("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        if (end.DayNumber - start.DayNumber + 1 > MaxRangeInDays)
        {
            return Error($"El rango no puede superar {MaxRangeInDays} días.");
        }

        IEnumerable<DateOnly> holidays = request.Festivos ?? HolidayParser.Parse(_configuration["Holidays"]);
        var businessDays = _calculator.Calculate(start, end, holidays);

        _logger.LogInformation("Días hábiles entre {Start} y {End}: {BusinessDays}", start, end, businessDays);

        return new OkObjectResult(new CalcularDiasHabilesResponse(start, end, businessDays));
    }

    private static BadRequestObjectResult Error(string message) => new(new { error = message });
}
