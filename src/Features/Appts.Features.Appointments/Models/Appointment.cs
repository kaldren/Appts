namespace Appts.Features.Appointments.Models;
public class Appointment
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}