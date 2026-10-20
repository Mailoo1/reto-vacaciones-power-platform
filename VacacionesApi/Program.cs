// =====================================================================================
// Program.cs - PUNTO DE ENTRADA DE LA API
// -------------------------------------------------------------------------------------
// Es lo primero que se ejecuta cuando arranca la Azure Function. Aquí se "arma" la
// aplicación: se le dice a Azure Functions cómo debe funcionar y qué servicios
// (clases de apoyo) puede usar nuestro código.
// =====================================================================================
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VacacionesApi.Services;

var host = new HostBuilder()
    // Modelo "isolated worker" de .NET 8: nuestro código corre en su propio proceso y
    // usa los tipos normales de ASP.NET Core (HttpRequest, IActionResult, etc.).
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Inyección de dependencias: cada vez que una clase pida un IBusinessDaysCalculator,
        // .NET le entrega el BusinessDaysCalculator. Así la función HTTP no depende de la
        // implementación concreta y es fácil de probar o de cambiar.
        // Singleton = se crea una sola vez y se reutiliza (no guarda estado, es seguro).
        services.AddSingleton<IBusinessDaysCalculator, BusinessDaysCalculator>();
    })
    .Build();

// Deja la API escuchando peticiones.
host.Run();
