using System;

namespace workstation_backend.ContractsContext.Domain.Models.Entities;

public class Clause
{
    public Guid Id { get; private set; }
    public Guid ContractId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public bool Mandatory { get; private set; }

    public Clause(Guid ContractId, string name, string content, int order, bool mandatory)
    {
        this.ContractId = ContractId;
        Name = name;
        Content = content;
        Order = order;
        Mandatory = mandatory;
    }
    private Clause() { }


}
