namespace workstation_backend.ContractsContext.Domain.Models.ValueObjects;

public record Money(decimal Value)
{
    public static Money operator +(Money a, Money b) => new Money(a.Value + b.Value);
    public static Money operator -(Money a, Money b) => new Money(a.Value - b.Value);
}
