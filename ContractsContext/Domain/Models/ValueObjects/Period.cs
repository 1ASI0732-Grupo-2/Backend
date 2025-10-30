namespace workstation_backend.ContractsContext.Domain.Models.ValueObjects;

public record Period(DateOnly Start, DateOnly End)
{
    public int TotalDays => End.DayNumber - Start.DayNumber;
    public bool IsActive(DateOnly date) => date >= Start && date <= End;
}
