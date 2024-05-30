using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appts.API;

public class ApptsDbContext : IdentityDbContext<ApplicationUser>
{
    public ApptsDbContext(DbContextOptions<ApptsDbContext> options)
        : base(options)
    {
    }
}

public class ApplicationUser : IdentityUser
{
}