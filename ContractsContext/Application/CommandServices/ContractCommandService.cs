using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.ContractsContext.Application.CommandServices;

public class ContractCommandService(IContractRepository contractRepository, IUnitOfWork unitOfWork) : IContractCommandService
{
    private readonly IContractRepository _contractRepository =
        contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));


    public async Task<Contract> Handle(CreateContractCommand command)
    {
        // Validar que no existe contrato activo para la oficina
        var activeContract = await _contractRepository.GetActiveContractByOfficeIdAsync(command.OfficeId);
        if (activeContract != null)
            throw new InvalidOperationException("Una misma oficina no puede tener dos contratos activos.");

        var contract = new Contract(
            command.OfficeId,
            command.OwnerId,
            command.RenterId,
            command.Description,
            command.StartDate,
            command.EndDate,
            command.BaseAmount,
            command.LateFee,
            command.InterestRate
        );

        await _contractRepository.AddAsync(contract);
        await _unitOfWork.CompleteAsync();

        return contract;
    }

    public async Task<Clause> Handle(AddClauseCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        var clause = new Clause(
            command.ContractId,
            command.Name,
            command.Content,
            command.Order,
            command.Mandatory
        );

        contract.AddClause(clause);
        await _unitOfWork.CompleteAsync();

        return clause;
    }

    public async Task<Compensation> Handle(AddCompensationCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        var compensation = new Compensation(
            command.ContractId,
            command.IssuerId,
            command.ReceiverId,
            command.Amount,
            command.Reason
        );

        contract.AddCompensation(compensation);
        await _unitOfWork.CompleteAsync();

        return compensation;
    }

    public async Task<Guid> Handle(ActivateContractCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        contract.Activate();
        await _unitOfWork.CompleteAsync();

        return contract.Id;
    }

    public async Task<Contract> Handle(SignContractCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        var signature = new Signature(
            command.ContractId,
            command.SignerId,
            command.SignatureHash
        );

        contract.AddSignature(signature);
        await _unitOfWork.CompleteAsync();

        return contract;
    }

    public async Task<PaymentReceipt> Handle(UpdateReceiptCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        if (contract.Receipt == null)
            throw new InvalidOperationException("No hay recibo emitido para este contrato.");

        contract.Receipt.UpdateWithCompensations(command.CompensationAdjustments, command.Notes);
        await _unitOfWork.CompleteAsync();

        return contract.Receipt;
    }

    public async Task<Guid> Handle(FinishContractCommand command)
    {
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        contract.Terminate();
        await _unitOfWork.CompleteAsync();

        return contract.Id;
    }
}
