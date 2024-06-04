using Appts.Features.Appointments;
using Appts.Features.Appointments.Features;
using Appts.Features.Emails.Features;
using Appts.Features.Identity;
using FastEndpoints;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;


var mediatRAssemblies = new[]
{
  Assembly.GetAssembly(typeof(RegisterUserCommand)), // Identity feature
  Assembly.GetAssembly(typeof(SendEmail)), // Email feature
  Assembly.GetAssembly(typeof(CreateAppointment)), // Appointments feature
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
