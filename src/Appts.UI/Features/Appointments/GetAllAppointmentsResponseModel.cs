namespace Appts.UI.Features.Appointments;

public class GetAllAppointmentsResponseModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Guid HostId { get; set; }
    public Guid ClientId { get; set; }
}