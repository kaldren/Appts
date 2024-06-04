using Appts.Features.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appts.Features.Identity.Infrastructure;

public class ApptsDbContext : IdentityDbContext<ApplicationUser>
{
    public ApptsDbContext(DbContextOptions<ApptsDbContext> options)
        : base(options)
    {
    }
}