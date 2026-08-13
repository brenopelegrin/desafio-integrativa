using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using GerenciadorProcessos.Api.Middleware;
using GerenciadorProcessos.Application.Interfaces;
using GerenciadorProcessos.Application.Services;
using GerenciadorProcessos.Application.Validators;
using GerenciadorProcessos.Domain.Interfaces;
using GerenciadorProcessos.Infrastructure.Data;
using GerenciadorProcessos.Infrastructure.Repositories;
using GerenciadorProcessos.Shared.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS
var frontendDomainStr = builder.Configuration["FRONTEND_DOMAIN"];

var allowedOrigins = string.IsNullOrEmpty(frontendDomainStr)
    ? Array.Empty<string>()
    : frontendDomainStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// TimeProvider
builder.Services.AddSingleton<System.TimeProvider, BrazilianTimeProvider>();

// DbContext (apenas injeta quando não estiver no ambiente de teste)
// Para os testes, usamos InMemory
if (builder.Environment.EnvironmentName != "Testing")
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? "Host=db;Port=5432;Database=gerenciador_processos;Username=admin;Password=admin_password";
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Repositories e Services
builder.Services.AddScoped<IProcessoRepository, ProcessoRepository>();
builder.Services.AddScoped<IProcessoService, ProcessoService>();
builder.Services.AddScoped<IEntidadeLegalRepository, EntidadeLegalRepository>();
builder.Services.AddScoped<IEntidadeLegalService, EntidadeLegalService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProcessoDtoValidator>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Uniformiza as URLs pra ficarem em minúsculas
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();

// Configurações do Swagger
builder.Services.AddSwaggerGen(options =>
{
    var applicationXml = $"{typeof(GerenciadorProcessos.Application.DTOs.Requests.CreateProcessoDto).Assembly.GetName().Name}.xml";
    options.IncludeXmlComments(System.IO.Path.Combine(System.AppContext.BaseDirectory, applicationXml));

    var apiXml = $"{typeof(GerenciadorProcessos.Api.Controllers.ProcessosController).Assembly.GetName().Name}.xml";
    var apiXmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, apiXml);
    if (System.IO.File.Exists(apiXmlPath))
    {
        options.IncludeXmlComments(apiXmlPath);
    }
});
builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();
app.MapControllers();

// Apply migrations automatically for Docker startup, except when testing
if (app.Environment.EnvironmentName != "Testing")
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}

app.Run();

public partial class Program { }
