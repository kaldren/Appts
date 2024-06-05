using Appts.Features.Appointments;
using Appts.Features.Appointments.Models;
using Appts.Features.Emails.Features;
using Appts.Features.Identity;
using Appts.Features.Identity.Models;
using FastEndpoints;
using System.Reflection;

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

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseFastEndpoints();

app.Run();
