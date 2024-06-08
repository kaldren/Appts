using Appts.Features.Identity.Infrastructure;
using Appts.Features.Identity.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appts.Features.Identity;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

        services.AddAuthorizationBuilder();

        services.AddDbContext<ApptsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<ApplicationUser>()
         .AddRoles<IdentityRole>()
         .AddEntityFrameworkStores<ApptsDbContext>()
         .AddApiEndpoints();

        //services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
        //services.AddAuthenticationCookie(validFor: TimeSpan.FromDays(30));

        services.AddFastEndpoints();

        return services;
    }
}
