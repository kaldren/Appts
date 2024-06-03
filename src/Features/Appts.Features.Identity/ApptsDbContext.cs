using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appts.Features.Identity;

public class ApptsDbContext : IdentityDbContext<ApplicationUser>
{
    public ApptsDbContext(DbContextOptions<ApptsDbContext> options)
        : base(options)
    {
    }
}