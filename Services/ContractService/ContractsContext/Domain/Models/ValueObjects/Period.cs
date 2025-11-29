namespace workstation_backend.ContractsContext.Domain.Models.ValueObjects;

public record Period(DateTime Start, DateTime End)
{
    public int TotalDays => (End - Start).Days + 1;
    public bool IsActive(DateTime date) => date >= Start && date <= End;
}
