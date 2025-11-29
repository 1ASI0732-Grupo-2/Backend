namespace workstation_backend.ContractsContext.Interface.Resources;

public record class AddClauseResource(    string Name,
    string Content,
    int Order,
    bool Mandatory);

