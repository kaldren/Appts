using Appts.Features.Appointments.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Appts.Features.Appointments.Infrastructure;
public class AppointmentsDb : IAppointmentsDb
{
    private readonly AppointmentsDbContext _dbContext;

    public AppointmentsDb(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            _dbContext.Appointments.Add(appointment);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
        return false;
    }

    public async Task<bool> AppointmentExistsAsync(Expression<Func<Appointment, bool>> predicate, CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            return await _dbContext.Appointments.AnyAsync(predicate);
        }
        return false;
    }
}
