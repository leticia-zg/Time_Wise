using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Extensions.Hosting;
using TimeWise.Data.Context;
using TimeWise.Data.Repositories;
using TimeWise.Core.Interfaces;
using TimeWise.Service.Services;
using TimeWise.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// API Versioning para Swagger
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    // Configuração básica do Swagger para v1
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1.0",
        Title = "TimeWise API",
        Description = "O TimeWise é uma aplicação que permite que usuários criem e gerenciem hábitos como pausas, postura e hidratação durante o trabalho."
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    // Adicionar exemplos e descrições (requer Swashbuckle.AspNetCore.Annotations)
    // options.EnableAnnotations(); // Comentado - requer pacote adicional, mas não é essencial
    
    // Configurar esquemas para evitar conflitos de nomes
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    
    // Agrupar por nome do controller (sem tags extras)
    options.TagActionsBy(api =>
    {
        var controllerName = api.ActionDescriptor?.RouteValues?.ContainsKey("controller") == true
            ? api.ActionDescriptor.RouteValues["controller"]
            : "Default";
        return new[] { controllerName };
    });
    
    // Incluir todos os endpoints sem agrupar por versão
    options.DocInclusionPredicate((name, api) => true);
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// DbContext - Oracle
var conn = configuration.GetConnectionString("OracleConnection") ?? configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TimeWiseDbContext>(options =>
    options.UseOracle(conn)
);

// DI - Repos and Services
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();

// Health Checks
var connectionString = configuration.GetConnectionString("OracleConnection") ?? configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString,
        name: "Banco de dados Oracle",
        tags: new[] { "db", "oracle", "ready" })
    .AddCheck("API", () =>
        HealthCheckResult.Healthy("API funcionando normalmente"),
        tags: new[] { "api", "live" });

// OpenTelemetry Tracing (console)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });

// Logging already configured by default
var app = builder.Build();

// Tratamento de exceções global
app.UseMiddleware<TimeWise.API.Middleware.ExceptionHandlingMiddleware>();

// Swagger sempre habilitado para documentação completa
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeWise API");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "TimeWise API - Documentação";
    options.DefaultModelsExpandDepth(-1);
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
    options.EnableValidator();
    // Não agrupar por tags
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { } // for integration tests
