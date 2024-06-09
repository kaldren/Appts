using Appts.Features.Appointments;
using Appts.Features.Appointments.Models;
using Appts.Features.Emails.Features;
using Appts.Features.Identity;
using Appts.Features.Identity.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;


var mediatRAssemblies = new[]
{
  Assembly.GetAssembly(typeof(ApplicationUser)), // Identity feature
  Assembly.GetAssembly(typeof(SendEmail)), // Email feature
  Assembly.GetAssembly(typeof(Appointment)), // Appointments feature
};

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

builder.Services.AddIdentityServices(Configuration);
builder.Services.AddAppointmentsServices(Configuration);

// Add a CORS policy for the client
builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins("https://localhost:7282", "https://localhost:7120")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

// Add NSwag services
builder.Services.AddOpenApiDocument();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    // Add OpenAPI/Swagger generator and the Swagger UI
    app.UseOpenApi();
    app.UseSwaggerUi(); // UseSwaggerUI Protected by if (env.IsDevelopment())
}

// Configure the HTTP request pipeline.

// Create routes for the identity endpoints
app.MapIdentityApi<ApplicationUser>();

app.UseCors("wasm");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager, [Microsoft.AspNetCore.Mvc.FromBody] object empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }

    return Results.Unauthorized();
}).RequireAuthorization();

app.MapGet("/roles", (ClaimsPrincipal user) =>
{
    if (user.Identity is not null && user.Identity.IsAuthenticated)
    {
        var identity = (ClaimsIdentity)user.Identity;
        var roles = identity.FindAll(identity.RoleClaimType)
            .Select(c =>
                new
                {
                    c.Issuer,
                    c.OriginalIssuer,
                    c.Type,
                    c.Value,
                    c.ValueType
                });

        return TypedResults.Json(roles);
    }

    return Results.Unauthorized();
}).RequireAuthorization();

app.UseFastEndpoints();

app.Run();
