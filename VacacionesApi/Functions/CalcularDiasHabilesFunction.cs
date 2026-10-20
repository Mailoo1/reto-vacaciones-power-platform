// =====================================================================================
// CalcularDiasHabilesFunction.cs - EL ENDPOINT HTTP DE LA API
// -------------------------------------------------------------------------------------
// Esta es la "puerta de entrada" de la API: recibe la petición que manda Power Apps (a
// través del Custom Connector), valida los datos, calcula los días hábiles y devuelve
// la respuesta en JSON.
//
// Esta clase NO hace las cuentas. Solo coordina:
//   1) lee y valida la petición,
//   2) decide de dónde salen los festivos,
//   3) le pide el cálculo a IBusinessDaysCalculator,
//   4) arma la respuesta.
// =====================================================================================
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
    // Límite de seguridad: evita pedir rangos absurdos (por ejemplo, 50 años) que harían
    // trabajar a la API de más. 366 días = un año completo, suficiente para vacaciones.
    private const int MaxRangeInDays = 366;

    private readonly IBusinessDaysCalculator _calculator;   // quien hace la cuenta
    private readonly IConfiguration _configuration;         // para leer la variable "Holidays"
    private readonly ILogger<CalcularDiasHabilesFunction> _logger; // para dejar registro en Azure

    // .NET entrega estas tres piezas automáticamente (inyección de dependencias).
    public CalcularDiasHabilesFunction(
        IBusinessDaysCalculator calculator,
        IConfiguration configuration,
        ILogger<CalcularDiasHabilesFunction> logger)
    {
        _calculator = calculator;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/dias-habiles
    /// AuthorizationLevel.Function significa que hay que enviar la clave de la función
    /// (encabezado x-functions-key). Sin clave, Azure responde 401 y el código ni se ejecuta.
    /// </summary>
    [Function("CalcularDiasHabiles")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "dias-habiles")] HttpRequest req)
    {
        // PASO 1 - Leer el JSON del cuerpo de la petición.
        // Si el JSON está mal formado, o una fecha no tiene el formato yyyy-MM-dd, la lectura
        // falla y respondemos 400 con un mensaje claro (en vez de dejar caer la API con un 500).
        CalcularDiasHabilesRequest? request;
        try
        {
            request = await req.ReadFromJsonAsync<CalcularDiasHabilesRequest>();
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return Error("El cuerpo debe ser un JSON válido con fechas en formato yyyy-MM-dd.");
        }

        // PASO 2 - Validar que vengan las dos fechas obligatorias.
        if (request?.FechaInicio is null || request.FechaFin is null)
        {
            return Error("Los campos 'fechaInicio' y 'fechaFin' son obligatorios.");
        }

        var start = request.FechaInicio.Value;
        var end = request.FechaFin.Value;

        // PASO 3 - Validar la lógica del rango.
        if (end < start)
        {
            return Error("La fecha de fin no puede ser anterior a la fecha de inicio.");
        }

        // +1 porque el rango es inclusivo (cuenta el día de inicio y el de fin).
        if (end.DayNumber - start.DayNumber + 1 > MaxRangeInDays)
        {
            return Error($"El rango no puede superar {MaxRangeInDays} días.");
        }

        // PASO 4 - Decidir de dónde salen los festivos (así NO están escritos en el código):
        //   a) Si la petición trae "festivos", se usan esos (tienen prioridad).
        //   b) Si no, se leen de la variable de aplicación "Holidays" configurada en Azure,
        //      que es un texto como "2026-11-02,2026-11-16". Cambiar los festivos = cambiar
        //      esa variable en el portal, sin tocar ni volver a publicar el código.
        IEnumerable<DateOnly> holidays = request.Festivos ?? HolidayParser.Parse(_configuration["Holidays"]);

        // PASO 5 - Calcular.
        var businessDays = _calculator.Calculate(start, end, holidays);

        // Deja constancia en los registros de Azure (útil para diagnosticar problemas).
        _logger.LogInformation("Días hábiles entre {Start} y {End}: {BusinessDays}", start, end, businessDays);

        // PASO 6 - Responder 200 con el JSON { fechaInicio, fechaFin, diasHabiles }.
        return new OkObjectResult(new CalcularDiasHabilesResponse(start, end, businessDays));
    }

    // Respuesta de error uniforme: HTTP 400 con { "error": "mensaje" }.
    // Power Apps lee ese mensaje y se lo muestra al usuario (FirstError.Message).
    private static BadRequestObjectResult Error(string message) => new(new { error = message });
}