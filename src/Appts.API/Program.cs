using Appts.UserManagement.Application.Commands;
using Appts.UserManagement.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;


var mediatRAssemblies = new[]
{
  Assembly.GetAssembly(typeof(RegisterUserCommand)), // UserManagement
};

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies));

builder.Services.AddIdentityServices(Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
