using System;
using ContractsContext.Domain.Models.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Enums;
using workstation_backend.ContractsContext.Domain.Models.Events;
using workstation_backend.ContractsContext.Domain.Models.Validators;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.ContractsContext.Application.CommandServices
{
    /// <summary>
    /// Servicio encargado de procesar comandos relacionados a la creación, modificación
    /// y finalización de contratos. 
    /// 
    /// Este servicio ejecuta validaciones, actualiza el estado del dominio, 
    /// persiste los cambios y emite eventos correspondientes para otros bounded contexts.
    /// </summary>
    public class ContractCommandService : IContractCommandService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ContractCommandService> _logger;

        private readonly IValidator<CreateContractCommand> _createContractValidator;
        private readonly IValidator<AddClauseCommand> _addClauseValidator;
        private readonly IValidator<AddCompensationCommand> _addCompensationValidator;
        private readonly IValidator<SignContractCommand> _signContractValidator;
        private readonly IValidator<UpdateReceiptCommand> _updateReceiptValidator;
        private readonly IValidator<FinishContractCommand> _finishContractValidator;
        private readonly IContractEventService _contractEventService;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de comandos del dominio de contratos.
        /// </summary>
        public ContractCommandService(
            IContractRepository contractRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateContractCommand> createContractValidator,
            IValidator<AddClauseCommand> addClauseValidator,
            IValidator<AddCompensationCommand> addCompensationValidator,
            IValidator<SignContractCommand> signContractValidator,
            IValidator<UpdateReceiptCommand> updateReceiptValidator,
            IValidator<FinishContractCommand> finishContractValidator,
            IContractEventService contractEventService,
            ILogger<ContractCommandService> logger)
        {
            _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _createContractValidator = createContractValidator;
            _addClauseValidator = addClauseValidator;
            _addCompensationValidator = addCompensationValidator;
            _signContractValidator = signContractValidator;
            _updateReceiptValidator = updateReceiptValidator;
            _finishContractValidator = finishContractValidator;

            _contractEventService = contractEventService ?? throw new ArgumentNullException(nameof(contractEventService));
        }

        /// <summary>
        /// Maneja la creación de un nuevo contrato en el sistema.
        /// </summary>
        /// <param name="command">Comando que contiene los datos del contrato a crear.</param>
        /// <returns>El contrato creado.</returns>
        /// <exception cref="ValidationException">Si los datos suministrados no son válidos.</exception>
        /// <exception cref="InvalidOperationException">Si la oficina ya posee un contrato activo.</exception>
        public async Task<Contract> Handle(CreateContractCommand command)
        {
            var validationResult = await _createContractValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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

        /// <summary>
        /// Maneja la adición de una cláusula a un contrato existente.
        /// </summary>
        /// <param name="command">Comando con los datos de la cláusula a agregar.</param>
        /// <returns>La cláusula creada.</returns>
        /// <exception cref="ValidationException">Si los datos del comando no son válidos.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        public async Task<Clause> Handle(AddClauseCommand command)
        {
            _logger.LogInformation("=== INICIANDO Handle(AddClauseCommand) ===");
            _logger.LogInformation($"ContractId: {command.ContractId}, Clause: {command.Name}");

            var validationResult = await _addClauseValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validación de cláusula fallida");
                throw new ValidationException(validationResult.Errors);
            }

            _logger.LogInformation($"Obteniendo contrato: {command.ContractId}");
            var contract = await _contractRepository.GetByIdAsync(command.ContractId)
                ?? throw new KeyNotFoundException($"Contract {command.ContractId} not found.");

            _logger.LogInformation($"Contrato encontrado. Clauses actuales: {contract.Clauses.Count}");

            var clause = new Clause(
                command.ContractId,
                command.Name,
                command.Content,
                command.Order,
                command.Mandatory
            );

            contract.AddClause(clause);
            _logger.LogInformation($"Cláusula agregada. Total clauses: {contract.Clauses.Count}");

            await _contractRepository.SaveChangesAsync();
            _logger.LogInformation("Cambios guardados");

            var @event = new ClauseAddedEvent(
                contract.Id,
                clause.Id,
                clause.Name,
                clause.Mandatory,
                DateTime.UtcNow
            );

            await _contractEventService.PublishAsync(@event);
            _logger.LogInformation("=== COMPLETADO Handle(AddClauseCommand) ===");

            return clause;
        }

        /// <summary>
        /// Maneja la creación de una compensación económica vinculada a un contrato.
        /// </summary>
        /// <param name="command">Comando con los detalles de la compensación.</param>
        /// <returns>La compensación creada.</returns>
        /// <exception cref="ValidationException">Si el comando no es válido.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        public async Task<Compensation> Handle(AddCompensationCommand command)
        {
            var validationResult = await _addCompensationValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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
            
            await _contractRepository.SaveChangesAsync();

            return compensation;
        }

        /// <summary>
        /// Maneja la activación de un contrato.
        /// </summary>
        /// <param name="command">Comando con el identificador del contrato a activar.</param>
        /// <returns>El ID del contrato activado.</returns>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
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

        /// <summary>
        /// Maneja la firma de un contrato por un usuario.
        /// </summary>
        /// <param name="command">Comando con los datos de la firma.</param>
        /// <returns>La firma creada.</returns>
        /// <exception cref="ValidationException">Si el comando no es válido.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        public async Task<Signature> Handle(SignContractCommand command)
        {
            //_logger.LogInformation("iniciando");
            //_logger.LogInformation($"ContractId: {command.ContractId}, SignerId: {command.SignerId}");

            try
            {
                //_logger.LogInformation("Validando comando...");
                var validationResult = await _signContractValidator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    //_logger.LogWarning("Validación fallida");
                    throw new ValidationException(validationResult.Errors);
                }
                //_logger.LogInformation("Comando válido");

                //_logger.LogInformation($"Obteniendo contrato con ID: {command.ContractId}");
                var contract = await _contractRepository.GetByIdAsync(command.ContractId);
                
                if (contract == null)
                {
                    //_logger.LogError($"Contrato no encontrado: {command.ContractId}");
                    throw new KeyNotFoundException($"Contract {command.ContractId} not found.");
                }
                //_logger.LogInformation($"Contrato encontrado. Estado: {contract.Status}, Signatures actuales: {contract.Signatures.Count}");

                //_logger.LogInformation("Creando nueva firma...");
                var signature = new Signature(
                    command.ContractId,
                    command.SignerId,
                    command.SignatureHash
                );
                //_logger.LogInformation($"Firma creada con ID: {signature.Id}");

                //_logger.LogInformation("Agregando firma explícitamente al contexto (INSERT)...");
                await _contractRepository.AddSignatureAsync(signature);
                //_logger.LogInformation($"Firma agregada al contexto");

                //_logger.LogInformation("5. Guardando SOLO la firma en la BD...");
                await _contractRepository.SaveChangesAsync();
                //_logger.LogInformation("Firma guardada exitosamente");

                //_logger.LogInformation("Agregando firma a la colección del contrato...");
                contract.AddSignature(signature);
                //_logger.LogInformation($"Firma agregada a colección. Total: {contract.Signatures.Count}");

                //_logger.LogInformation("Publicando evento...");
                var @event = new ContractSignedEvent(
                    contract.Id,
                    signature.SignerId,
                    signature.SignatureHash,
                    DateTime.UtcNow,
                    true
                );
                await _contractEventService.PublishAsync(@event);
                //_logger.LogInformation("✓ Evento publicado");

                //_logger.LogInformation("=== COMPLETADO Handle(SignContractCommand) exitosamente ===");
                return signature;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error en Handle(SignContractCommand): {ex.Message}");
                throw new Exception(
                    $"Error al firmar contrato. ContractId={command.ContractId}, SignerId={command.SignerId}. " +
                    $"Detalle: {ex.Message}",
                    ex
                );
            }
        }
        /// <summary>
        /// Maneja la actualización del recibo de pago de un contrato.
        /// </summary>
        /// <param name="command">Comando con los ajustes y notas del recibo.</param>
        /// <returns>El recibo actualizado.</returns>
        /// <exception cref="ValidationException">Si el comando no es válido.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        /// <exception cref="InvalidOperationException">Si el contrato no tiene un recibo emitido.</exception>
        public async Task<PaymentReceipt> Handle(UpdateReceiptCommand command)
        {
            var validationResult = await _updateReceiptValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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

        /// <summary>
        /// Maneja la finalización de un contrato.
        /// </summary>
        /// <param name="command">Comando con el motivo de finalización.</param>
        /// <returns>El contrato finalizado.</returns>
        /// <exception cref="ValidationException">Si el comando no es válido.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        public async Task<Contract> Handle(FinishContractCommand command)
        {
            var validationResult = await _finishContractValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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
}
