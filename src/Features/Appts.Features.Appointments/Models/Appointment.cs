namespace Appts.Features.Appointments.Models;
public class Appointment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; set; } = null!;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string OwnerId { get; set; }
    public string ClientId { get; set; }
}