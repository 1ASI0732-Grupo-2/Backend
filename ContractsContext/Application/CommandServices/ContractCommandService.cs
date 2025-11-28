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
            IContractEventService contractEventService)
        {
            _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

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
            var validationResult = await _addClauseValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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
            await _unitOfWork.CompleteAsync();

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
        /// <returns>El contrato actualizado.</returns>
        /// <exception cref="ValidationException">Si el comando no es válido.</exception>
        /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
        public async Task<Contract> Handle(SignContractCommand command)
        {
            Console.WriteLine($"\n{'=' * 60}");
            Console.WriteLine("INICIO SignContractCommand");
            Console.WriteLine($"{'=' * 60}");
            Console.WriteLine($"📥 ContractId: {command.ContractId}");
            Console.WriteLine($"📥 SignerId: {command.SignerId}");
            Console.WriteLine($"📥 SignatureHash: {command.SignatureHash}");

            try
            {
                // VALIDACIÓN
                Console.WriteLine("\n🔍 Iniciando validación...");
                var validationResult = await _signContractValidator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    Console.WriteLine("❌ Validación FALLIDA:");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.WriteLine($"   - {error.PropertyName}: {error.ErrorMessage}");
                    }
                    throw new ValidationException(validationResult.Errors);
                }
                Console.WriteLine("✅ Validación OK");

                // OBTENER CONTRATO
                Console.WriteLine($"\n📂 Obteniendo contrato {command.ContractId}...");
                var contract = await _contractRepository.GetByIdAsync(command.ContractId);

                if (contract == null)
                {
                    Console.WriteLine($"❌ Contrato {command.ContractId} NO ENCONTRADO");
                    throw new KeyNotFoundException($"Contract {command.ContractId} not found.");
                }

                Console.WriteLine("✅ Contrato encontrado:");
                Console.WriteLine($"   Id: {contract.Id}");
                Console.WriteLine($"   Status: {contract.Status}");
                Console.WriteLine($"   OwnerId: {contract.OwnerId}");
                Console.WriteLine($"   RenterId: {contract.RenterId}");

                // VERIFICAR COLECCIONES
                Console.WriteLine($"\n🔍 Verificando colecciones:");
                Console.WriteLine($"   Signatures: {(contract.Signatures == null ? "NULL" : $"Count={contract.Signatures.Count}")}");
                Console.WriteLine($"   Clauses: {(contract.Clauses == null ? "NULL" : $"Count={contract.Clauses.Count}")}");
                Console.WriteLine($"   Compensations: {(contract.Compensations == null ? "NULL" : $"Count={contract.Compensations.Count}")}");

                // CREAR FIRMA
                Console.WriteLine($"\n🖊️ Creando nueva firma...");
                var signature = new Signature(
                    command.ContractId,
                    command.SignerId,
                    command.SignatureHash
                );
                Console.WriteLine($"✅ Firma creada:");
                Console.WriteLine($"   Id: {signature.Id}");
                Console.WriteLine($"   SignerId: {signature.SignerId}");
                Console.WriteLine($"   ContractId: {signature.ContractId}");

                // AGREGAR FIRMA AL CONTRATO
                Console.WriteLine($"\n➕ Agregando firma al contrato...");
                Console.WriteLine($"   Antes de AddSignature - Signatures count: {contract.Signatures?.Count ?? 0}");

                contract.AddSignature(signature);

                Console.WriteLine($"   Después de AddSignature - Signatures count: {contract.Signatures?.Count ?? 0}");
                Console.WriteLine("✅ Firma agregada al contrato");

                // GUARDAR
                Console.WriteLine($"\n💾 Guardando cambios en la base de datos...");
                await _unitOfWork.CompleteAsync();
                Console.WriteLine("✅ Cambios guardados");

                // PUBLICAR EVENTO
                Console.WriteLine($"\n📢 Publicando evento ContractSignedEvent...");
                var @event = new ContractSignedEvent(
                    contract.Id,
                    signature.SignerId,
                    signature.SignatureHash,
                    DateTime.UtcNow,
                    true
                );
                await _contractEventService.PublishAsync(@event);
                Console.WriteLine("✅ Evento publicado");

                Console.WriteLine($"\n{'=' * 60}");
                Console.WriteLine("FIN SignContractCommand - SUCCESS");
                Console.WriteLine($"{'=' * 60}\n");

                return contract;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n{'=' * 60}");
                Console.WriteLine("❌❌❌ EXCEPCIÓN EN SignContractCommand ❌❌❌");
                Console.WriteLine($"{'=' * 60}");
                Console.WriteLine($"Tipo: {ex.GetType().Name}");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"\nStack Trace:");
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"\n--- Inner Exception ---");
                    Console.WriteLine($"Tipo: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack Trace:");
                    Console.WriteLine(ex.InnerException.StackTrace);
                }

                Console.WriteLine($"{'=' * 60}\n");
                throw;
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
