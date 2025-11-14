using System;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Entities;

/// <summary>
    /// Representa una compensación económica relacionada a un contrato entre dos partes
    /// </summary>
    public class Compensation
    {
        /// <summary>
        /// Identificador único de la compensación
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador del contrato asociado a esta compensación
        /// </summary>
        /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa7</example>
        public Guid ContractId { get; private set; }

        /// <summary>
        /// Identificador del usuario que emite la compensación
        /// </summary>
        /// <example>5fa85f64-5717-4562-b3fc-2c963f66afa8</example>
        public Guid IssuerId { get; private set; }

        /// <summary>
        /// Identificador del usuario que recibe la compensación
        /// </summary>
        /// <example>6fa85f64-5717-4562-b3fc-2c963f66afa9</example>
        public Guid ReceiverId { get; private set; }

        /// <summary>
        /// Monto de la compensación en la moneda del contrato
        /// </summary>
        /// <example>1500.50</example>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Razón o motivo por el cual se emite la compensación
        /// </summary>
        /// <example>Pago por incumplimiento de cláusula de entrega</example>
        public string Reason { get; private set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de creación de la compensación
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Estado actual de la compensación (Pending, Approved, Rejected)
        /// </summary>
        /// <example>Pending</example>
        public CompensationStatus Status { get; private set; } = CompensationStatus.Pending;

        /// <summary>
        /// Constructor para crear una nueva compensación
        /// </summary>
        /// <param name="contractId">ID del contrato asociado</param>
        /// <param name="issuerId">ID del usuario emisor</param>
        /// <param name="receiverId">ID del usuario receptor</param>
        /// <param name="amount">Monto de la compensación</param>
        /// <param name="reason">Motivo de la compensación</param>
        public Compensation(Guid contractId, Guid issuerId, Guid receiverId, decimal amount, string reason)
        {
            ContractId = contractId;
            IssuerId = issuerId;
            ReceiverId = receiverId;
            Amount = amount;
            Reason = reason;
        }

        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private Compensation() { }

        /// <summary>
        /// Aprueba la compensación cambiando su estado a Approved
        /// </summary>
        public void Approve() => Status = CompensationStatus.Approved;

        /// <summary>
        /// Rechaza la compensación cambiando su estado a Rejected
        /// </summary>
        public void Reject() => Status = CompensationStatus.Rejected;
    }
