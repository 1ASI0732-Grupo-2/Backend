using MediatR;
using workstation_backend.ContractsContext.Domain.Models.Entities;

namespace workstation_backend.ContractsContext.Domain.Models.Queries;

public record GetPaymentReceiptByContractIdQuery(Guid ContractId) : IRequest<PaymentReceipt?>;
