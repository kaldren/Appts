using Appts.Features.Appointments.Models;
using Microsoft.EntityFrameworkCore;

namespace Appts.Features.Appointments;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; } = null!;
}