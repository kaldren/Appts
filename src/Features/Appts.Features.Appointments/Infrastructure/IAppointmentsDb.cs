using Appts.Features.Appointments.Models;
using System.Linq.Expressions;

namespace Appts.Features.Appointments.Infrastructure;

public interface IAppointmentsDb
{
    Task<bool> AppointmentExistsAsync(Expression<Func<Appointment, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken);
}
