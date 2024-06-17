using Appts.Features.Appointments.Infrastructure;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appts.Features.Appointments;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppointmentsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppointmentsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddFastEndpoints();
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        services.AddScoped<IAppointmentsDb, AppointmentsDb>();

        return services;
    }
}
