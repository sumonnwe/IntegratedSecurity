// Minimal API with Swagger + JWT bearer auth.
// Register RabbitMqEventBus and CardScanHandler.
// POST /card-scan { cardNo, readerId } -> 200/401; GET /health.
using IntegratedSecurity.Application.UseCases;
using IntegratedSecurity.Application.Abstractions;
using IntegratedSecurity.Infrastructure.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var b = WebApplication.CreateBuilder(args);
b.Services.Configure<RabbitMqOptions>(b.Configuration.GetSection("RabbitMq"));
b.Services.AddSingleton<IEventBus, RabbitMqEventBus>();
b.Services.AddScoped<CardScanHandler>();
b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
b.Services.AddAuthorization();
b.Services.AddEndpointsApiExplorer();
b.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Integrated Security API", Version = "v1" }));
var app = b.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.UseAuthentication(); app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { ok = true }));
app.MapPost("/card-scan", async (CardScanHandler h, CardScanRequest req, CancellationToken ct) =>
{
    var ok = await h.HandleAsync(req.CardNo, req.ReaderId, ct);
    return ok ? Results.Ok(new { authorized = true }) : Results.Unauthorized();
});

app.Run();

public record CardScanRequest(string CardNo, string ReaderId);
