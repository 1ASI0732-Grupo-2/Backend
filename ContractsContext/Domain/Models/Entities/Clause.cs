using System;

namespace workstation_backend.ContractsContext.Domain.Models.Entities
{
    /// <summary>
    /// Representa una cláusula dentro de un contrato
    /// </summary>
    public class Clause
    {
        /// <summary>
        /// Identificador único de la cláusula
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador del contrato al que pertenece esta cláusula
        /// </summary>
        /// <example>4fa85f64-5717-4562-b3fc-2c963f66afa7</example>
        public Guid ContractId { get; private set; }

        /// <summary>
        /// Nombre o título de la cláusula
        /// </summary>
        /// <example>Cláusula de Confidencialidad</example>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Contenido completo de la cláusula con todos sus términos y condiciones
        /// </summary>
        /// <example>Las partes acuerdan mantener confidencial toda información compartida durante la vigencia del contrato...</example>
        public string Content { get; private set; } = string.Empty;

        /// <summary>
        /// Número de orden que define la posición de la cláusula en el contrato
        /// </summary>
        /// <example>1</example>
        public int Order { get; private set; }

        /// <summary>
        /// Indica si la cláusula es obligatoria (true) u opcional (false)
        /// </summary>
        /// <example>true</example>
        public bool Mandatory { get; private set; }

        /// <summary>
        /// Constructor para crear una nueva cláusula
        /// </summary>
        /// <param name="ContractId">ID del contrato padre</param>
        /// <param name="name">Nombre de la cláusula</param>
        /// <param name="content">Contenido de la cláusula</param>
        /// <param name="order">Orden de aparición en el contrato</param>
        /// <param name="mandatory">Indica si es obligatoria</param>
        public Clause(Guid ContractId, string name, string content, int order, bool mandatory)
        {
            this.ContractId = ContractId;
            Name = name;
            Content = content;
            Order = order;
            Mandatory = mandatory;
        }

        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private Clause() { }
    }
}