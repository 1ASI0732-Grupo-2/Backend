using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record UpdateReceiptCommand(
    Guid ContractId,
    decimal CompensationAdjustments,
    string Notes
) : IRequest<Unit>;