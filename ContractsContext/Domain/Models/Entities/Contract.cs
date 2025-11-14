
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace ContractsContext.Domain.Models.Entities;
public class Contract
    {
        /// <summary>
        /// Identificador único del contrato
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador de la oficina que se está arrendando
        /// </summary>
        /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa7</example>
        public Guid OfficeId { get; private set; }

        /// <summary>
        /// Identificador del propietario de la oficina
        /// </summary>
        /// <example>5fa85f64-5717-4562-b3fc-2c963f66afa8</example>
        public Guid OwnerId { get; private set; }

        /// <summary>
        /// Identificador del arrendatario de la oficina
        /// </summary>
        /// <example>6fa85f64-5717-4562-b3fc-2c963f66afa9</example>
        public Guid RenterId { get; private set; }

        /// <summary>
        /// Descripción detallada de los términos y condiciones del contrato
        /// </summary>
        /// <example>Contrato de arrendamiento mensual con opción a renovación</example>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio de vigencia del contrato
        /// </summary>
        /// <example>2024-01-01T00:00:00Z</example>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Fecha de finalización del contrato
        /// </summary>
        /// <example>2024-12-31T23:59:59Z</example>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Monto base del alquiler mensual
        /// </summary>
        /// <example>1500.00</example>
        public decimal BaseAmount { get; private set; }

        /// <summary>
        /// Monto de la penalización por pago tardío
        /// </summary>
        /// <example>50.00</example>
        public decimal LateFee { get; private set; }

        /// <summary>
        /// Tasa de interés aplicable por mora (en porcentaje)
        /// </summary>
        /// <example>5.5</example>
        public decimal InterestRate { get; private set; }

        /// <summary>
        /// Estado actual del contrato (Draft, PendingSignatures, Active, Completed, Cancelled)
        /// </summary>
        /// <example>Draft</example>
        public ContractStatus Status { get; private set; } = ContractStatus.Draft;

        /// <summary>
        /// Fecha y hora de creación del contrato
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora en que el contrato fue activado
        /// </summary>
        /// <example>2024-01-20T15:45:00Z</example>
        public DateTime? ActivatedAt { get; private set; }

        /// <summary>
        /// Fecha y hora en que el contrato fue terminado o cancelado
        /// </summary>
        /// <example>2024-12-31T23:59:59Z</example>
        public DateTime? TerminatedAt { get; private set; }

        private readonly List<Clause> _clauses = new();
        /// <summary>
        /// Colección de cláusulas asociadas al contrato
        /// </summary>
        public IReadOnlyCollection<Clause> Clauses => _clauses.AsReadOnly();

        private readonly List<Signature> _signatures = new();
        /// <summary>
        /// Colección de firmas de las partes involucradas en el contrato
        /// </summary>
        public IReadOnlyCollection<Signature> Signatures => _signatures.AsReadOnly();

        private readonly List<Compensation> _compensations = new();
        /// <summary>
        /// Colección de compensaciones económicas asociadas al contrato
        /// </summary>
        public IReadOnlyCollection<Compensation> Compensations => _compensations.AsReadOnly();

        /// <summary>
        /// Recibo de pago asociado al contrato
        /// </summary>
        public PaymentReceipt? Receipt { get; private set; }

        /// <summary>
        /// Constructor para crear un nuevo contrato
        /// </summary>
        /// <param name="officeId">ID de la oficina a arrendar</param>
        /// <param name="ownerId">ID del propietario</param>
        /// <param name="renterId">ID del arrendatario</param>
        /// <param name="description">Descripción del contrato</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de finalización</param>
        /// <param name="baseAmount">Monto base de alquiler</param>
        /// <param name="lateFee">Penalización por mora</param>
        /// <param name="interestRate">Tasa de interés por mora</param>
        public Contract(Guid officeId, Guid ownerId, Guid renterId, string description, DateTime startDate, DateTime endDate, decimal baseAmount, decimal lateFee, decimal interestRate)
        {
            OfficeId = officeId;
            OwnerId = ownerId;
            RenterId = renterId;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            BaseAmount = baseAmount;
            LateFee = lateFee;
            InterestRate = interestRate;
        }

        /// <summary>
        /// Constructor sin parámetros para Entity Framework
        /// </summary>
        public Contract() { }

        /// <summary>
        /// Agrega una cláusula al contrato. Solo permitido en estado Draft o PendingSignatures
        /// </summary>
        /// <param name="clause">Cláusula a agregar</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el contrato ya está activo</exception>
        public void AddClause(Clause clause)
        {
            if (Status != ContractStatus.Draft && Status != ContractStatus.PendingSignatures)
                throw new InvalidOperationException("No se pueden agregar cláusulas después de que el contrato esté activo.");

            _clauses.Add(clause);
        }

        /// <summary>
        /// Agrega una firma al contrato y actualiza su estado si ambas partes han firmado
        /// </summary>
        /// <param name="signature">Firma a agregar</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el contrato ya está activo</exception>
        public void AddSignature(Signature signature)
        {
            if (Status == ContractStatus.Active)
                throw new InvalidOperationException("El contrato ya está activo.");

            _signatures.Add(signature);

            // Verificar si ambas partes han firmado
            var ownerSigned = _signatures.Any(s => s.SignerId == OwnerId);
            var renterSigned = _signatures.Any(s => s.SignerId == RenterId);

            if (ownerSigned && renterSigned && Status == ContractStatus.Draft)
            {
                Status = ContractStatus.PendingSignatures;
            }
        }

        /// <summary>
        /// Agrega una compensación al contrato. Solo permitido en contratos activos
        /// </summary>
        /// <param name="compensation">Compensación a agregar</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el contrato no está activo</exception>
        public void AddCompensation(Compensation compensation)
        {
            if (Status != ContractStatus.Active)
                throw new InvalidOperationException("Las compensaciones solo se permiten en contratos activos.");

            _compensations.Add(compensation);
        }

        /// <summary>
        /// Asigna un recibo de pago al contrato
        /// </summary>
        /// <param name="receipt">Recibo de pago a asignar</param>
        public void SetReceipt(PaymentReceipt receipt)
        {
            Receipt = receipt;
        }

        /// <summary>
        /// Activa el contrato. Requiere que ambas partes hayan firmado
        /// </summary>
        /// <exception cref="InvalidOperationException">Se lanza cuando el contrato no está en estado PendingSignatures o faltan firmas</exception>
        public void Activate()
        {
            if (Status != ContractStatus.PendingSignatures)
                throw new InvalidOperationException("El contrato debe estar pendiente de firmas.");

            var ownerSigned = _signatures.Any(s => s.SignerId == OwnerId);
            var renterSigned = _signatures.Any(s => s.SignerId == RenterId);

            if (!ownerSigned || !renterSigned)
                throw new InvalidOperationException("Ambas firmas son necesarias para activar el contrato.");

            Status = ContractStatus.Active;
            ActivatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Finaliza el contrato de manera normal. No permite compensaciones pendientes
        /// </summary>
        /// <exception cref="InvalidOperationException">Se lanza cuando existen compensaciones pendientes</exception>
        public void Terminate()
        {
            var hasPendingCompensations = _compensations.Any(c => c.Status == CompensationStatus.Pending);
            if (hasPendingCompensations)
                throw new InvalidOperationException("El contrato no puede finalizar con compensaciones pendientes.");

            Status = ContractStatus.Completed;
            TerminatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Cancela el contrato. Solo permitido en contratos no activos
        /// </summary>
        /// <exception cref="InvalidOperationException">Se lanza cuando se intenta cancelar un contrato activo</exception>
        public void Cancel()
        {
            if (Status == ContractStatus.Active)
                throw new InvalidOperationException("No se puede cancelar un contrato activo. Use Terminate en su lugar.");

            Status = ContractStatus.Cancelled;
            TerminatedAt = DateTime.UtcNow;
        }
    }