using Appts.UserManagement.Application.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appts.UserManagement.Infrastructure;
public class ApptsDbContext : IdentityDbContext<UserModel>
{
    public ApptsDbContext(DbContextOptions<ApptsDbContext> options)
        : base(options)
    {
    }
}