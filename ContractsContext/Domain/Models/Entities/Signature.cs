using System;

namespace workstation_backend.ContractsContext.Domain.Models.Entities
{
    /// <summary>
    /// Representa la firma digital asociada a un contrato,
    /// incluyendo el firmante, la fecha y el hash de la firma.
    /// </summary>
    public class Signature
    {
        /// <summary>
        /// Identificador único de la firma.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador del contrato al cual pertenece esta firma.
        /// </summary>
        public Guid ContractId { get; private set; }

        /// <summary>
        /// Identificador del usuario que realizó la firma.
        /// </summary>
        public Guid SignerId { get; private set; }

        /// <summary>
        /// Fecha y hora en la que se registró la firma.
        /// </summary>
        public DateTime SignedAt { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Hash o representación encriptada de la firma digital.
        /// </summary>
        public string SignatureHash { get; private set; } = string.Empty;

        /// <summary>
        /// Crea una nueva firma para un contrato específico.
        /// </summary>
        /// <param name="contractId">Identificador del contrato firmado.</param>
        /// <param name="signerId">Identificador del firmante.</param>
        /// <param name="signatureHash">Hash de la firma digital.</param>
        public Signature(Guid contractId, Guid signerId, string signatureHash)
        {
            ContractId = contractId;
            SignerId = signerId;
            SignatureHash = signatureHash;
        }

        private Signature() { }
    }
}
