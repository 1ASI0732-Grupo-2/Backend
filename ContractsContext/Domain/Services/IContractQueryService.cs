using System;
using System.Collections.Generic;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Queries;

namespace workstation_backend.ContractsContext.Domain.Services;

public interface IContractQueryService
{
    Task<IEnumerable<Contract>> Handle(GetActiveContractsQuery query);
    Task<Contract> Handle(GetContractByIdQuery query);
    Task<List<Contract?>> Handle(GetContractByUserIdQuery query);
    Task<PaymentReceipt> Handle(GetPaymentReceiptByContractIdQuery query);
    Task<List<Compensation>> Handle(GetCompensationsByContractIdQuery query);

}
