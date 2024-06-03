using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appts.UserManagement.Infrastructure;
public class ApptsDbContext : IdentityDbContext<ApplicationUser>
{
    public ApptsDbContext(DbContextOptions<ApptsDbContext> options)
        : base(options)
    {
    }
}