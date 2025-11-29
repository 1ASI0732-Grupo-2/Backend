namespace workstation_backend.ContractsContext.Interface.Resources;

public record class ClauseResource(    Guid Id,
    string Name,
    string Content,
    int Order,
    bool Mandatory);
