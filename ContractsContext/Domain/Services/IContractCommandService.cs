using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Entities;

namespace workstation_backend.ContractsContext.Domain.Services;

public interface IContractCommandService
{
    Task<Contract> Handle(CreateContractCommand command);
    Task<Clause> Handle(AddClauseCommand command);
    Task<Compensation> Handle(AddCompensationCommand command);
    Task<Guid> Handle(ActivateContractCommand command);

    Task<Contract> Handle(SignContractCommand command);
    Task<PaymentReceipt> Handle(UpdateReceiptCommand command);


}
