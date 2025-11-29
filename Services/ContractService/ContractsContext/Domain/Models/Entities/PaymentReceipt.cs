using System;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Entities
{
    /// <summary>
    /// Representa un comprobante de pago asociado a un contrato, incluyendo
    /// montos base, compensaciones y cambios de estado.
    /// </summary>
    public class PaymentReceipt
    {
        /// <summary>
        /// Identificador único del comprobante de pago.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador del contrato al que pertenece este comprobante.
        /// </summary>
        public Guid ContractId { get; private set; }

        /// <summary>
        /// Número del comprobante utilizado para el seguimiento interno o externo.
        /// </summary>
        public string ReceiptNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Monto base del pago antes de aplicar compensaciones.
        /// </summary>
        public decimal BaseAmount { get; private set; }

        /// <summary>
        /// Ajustes aplicados por compensaciones. Puede ser un valor positivo o negativo.
        /// </summary>
        public decimal CompensationAdjustments { get; private set; }

        /// <summary>
        /// Monto final calculado luego de aplicar las compensaciones.
        /// </summary>
        public decimal FinalAmount => BaseAmount + CompensationAdjustments;

        /// <summary>
        /// Fecha y hora en que se emitió el comprobante.
        /// </summary>
        public DateTime IssuedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha y hora de la última actualización del comprobante, si existe.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Notas o comentarios adicionales sobre el comprobante.
        /// </summary>
        public string Notes { get; private set; } = string.Empty;

        /// <summary>
        /// Estado actual del comprobante de pago.
        /// </summary>
        public PaymentReceiptStatus Status { get; private set; } = PaymentReceiptStatus.Issued;

        /// <summary>
        /// Crea un nuevo comprobante de pago asociado a un contrato.
        /// </summary>
        /// <param name="contractId">Identificador del contrato asociado.</param>
        /// <param name="receiptNumber">Número del comprobante.</param>
        /// <param name="baseAmount">Monto base antes de aplicar ajustes.</param>
        public PaymentReceipt(Guid contractId, string receiptNumber, decimal baseAmount)
        {
            ContractId = contractId;
            ReceiptNumber = receiptNumber;
            BaseAmount = baseAmount;
        }

        private PaymentReceipt() { }

        /// <summary>
        /// Actualiza el comprobante aplicando ajustes de compensación y registrando notas.
        /// </summary>
        /// <param name="adjustment">Monto del ajuste. Puede ser positivo o negativo.</param>
        /// <param name="note">Nota o detalle que explica el ajuste.</param>
        public void UpdateWithCompensations(decimal adjustment, string note)
        {
            CompensationAdjustments = adjustment;
            Status = PaymentReceiptStatus.Updated;
            UpdatedAt = DateTime.UtcNow;
            Notes = note;
        }

        /// <summary>
        /// Finaliza el comprobante, indicando que no se permitirán más cambios.
        /// </summary>
        public void FinalizeReceipt()
        {
            Status = PaymentReceiptStatus.Finalized;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Cancela el comprobante, marcándolo como inválido o anulado.
        /// </summary>
        public void Cancel()
        {
            Status = PaymentReceiptStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
