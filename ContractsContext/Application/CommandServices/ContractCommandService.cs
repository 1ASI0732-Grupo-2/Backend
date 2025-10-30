using System;
using ContractsContext.Domain.Models.Entities;
using FluentValidation;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Events;
using workstation_backend.ContractsContext.Domain.Models.Validators;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.ContractsContext.Application.CommandServices;

public class ContractCommandService(IContractRepository contractRepository, IUnitOfWork unitOfWork,
IValidator<CreateContractCommand> createContractValidator,
IValidator<AddClauseCommand> addClauseValidator,
    IValidator<AddCompensationCommand> addCompensationValidator,
    IValidator<SignContractCommand> signContractValidator,
    IValidator<UpdateReceiptCommand> updateReceiptValidator,
    IValidator<FinishContractCommand> finishContractValidator,
    IContractEventService contractEventService) : IContractCommandService
{
    private readonly IContractRepository _contractRepository =
        contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly IValidator<CreateContractCommand> _createContractValidator = createContractValidator;
    private readonly IValidator<AddClauseCommand> _addClauseValidator = addClauseValidator;
    private readonly IValidator<AddCompensationCommand> _addCompensationValidator = addCompensationValidator;
    private readonly IValidator<SignContractCommand> _signContractValidator = signContractValidator;
    private readonly IValidator<UpdateReceiptCommand> _updateReceiptValidator = updateReceiptValidator;
    private readonly IValidator<FinishContractCommand> _finishContractValidator = finishContractValidator;    
    
    private readonly IContractEventService _contractEventService = 
        contractEventService ?? throw new ArgumentNullException(nameof(contractEventService));

    public async Task<Contract> Handle(CreateContractCommand command)
    {
        var validationResult = await _createContractValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

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

         
        var @event = new ContractCreatedEvent(
        contract.Id,
        contract.OfficeId,
        contract.OwnerId,
        contract.RenterId,
        contract.StartDate,
        contract.EndDate,
        contract.BaseAmount,
        DateTime.UtcNow
        );


        await _contractEventService.PublishAsync(@event);

        return contract;
    }

    public async Task<Clause> Handle(AddClauseCommand command)
    {

        var validationResult = await _addClauseValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

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
        
        await _contractRepository.SaveChangesAsync(); 


        var @event = new ClauseAddedEvent(
            contract.Id,
            clause.Id,
            clause.Name,
            clause.Mandatory,
            DateTime.UtcNow
        );
        await _contractEventService.PublishAsync(@event);

        return clause;
    }

    public async Task<Compensation> Handle(AddCompensationCommand command)
    {
        var validationResult = await _addCompensationValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

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
        var @event = new ContractActivatedEvent(
            contract.Id,
            contract.OfficeId,
            DateTime.UtcNow
        );
        await _contractEventService.PublishAsync(@event);

        return contract.Id;
    }

    public async Task<Contract> Handle(SignContractCommand command)
    {
        var validationResult = await _signContractValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        var signature = new Signature(
            command.ContractId,
            command.SignerId,
            command.SignatureHash
        );

        contract.AddSignature(signature);
        await _unitOfWork.CompleteAsync();

        var @event = new ContractSignedEvent(
            contract.Id,
            signature.SignerId,
            signature.SignatureHash,
            DateTime.UtcNow,
            true
        );
        await _contractEventService.PublishAsync(@event);
        return contract;
    }

    public async Task<PaymentReceipt> Handle(UpdateReceiptCommand command)
    {
        var validationResult = await _updateReceiptValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        if (contract.Receipt == null)
            throw new InvalidOperationException("No hay recibo emitido para este contrato.");

        contract.Receipt.UpdateWithCompensations(command.CompensationAdjustments, command.Notes);
        await _unitOfWork.CompleteAsync();

        var @event = new ReceiptUpdatedEvent(
            contract.Receipt.Id,
            contract.Id,
            contract.Receipt.CompensationAdjustments,
            contract.Receipt.FinalAmount,
            contract.Receipt.Notes,
            DateTime.UtcNow
        );
        await _contractEventService.PublishAsync(@event);

        return contract.Receipt;
    }

    public async Task<Contract> Handle(FinishContractCommand command)
    {
        var validationResult = await _finishContractValidator.ValidateAsync(command);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        var contract = await _contractRepository.GetByIdAsync(command.ContractId)
            ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

        contract.Terminate();
        await _unitOfWork.CompleteAsync();
        var @event = new ContractFinishedEvent(
            contract.Id,
            contract.OfficeId,
            DateTime.UtcNow,
            command.Reason
        );
        await _contractEventService.PublishAsync(@event);

        return contract;
    }
}
