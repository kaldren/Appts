namespace Appts.UI.Features.Appointments;

public class GetAllAppointmentsResponseModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}