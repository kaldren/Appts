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

        return await Task.FromResult(false);
    }

    public async Task<List<Appointment>> GetAllClientAppointmentsAsync(string clientId, CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            return await _dbContext.Appointments.Where(x => x.ClientId == clientId).ToListAsync(cancellationToken);
        }

        return await Task.FromResult(new List<Appointment>());
    }

    public async Task<Appointment?> GetAppointmentByClientIdAsync(string clientId, CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            return await _dbContext.Appointments.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
        }

        return await Task.FromResult<Appointment>(null);
    }
}
