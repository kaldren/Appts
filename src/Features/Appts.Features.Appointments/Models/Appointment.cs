namespace Appts.Features.Appointments.Models;
public class Appointment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Guid HostId { get; set; }
    public Guid ClientId { get; set; }
}