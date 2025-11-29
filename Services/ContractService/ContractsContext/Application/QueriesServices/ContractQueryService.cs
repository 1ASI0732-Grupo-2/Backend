using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Queries;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Application.QueriesServices;

/// <summary>
/// Servicio encargado de procesar consultas relacionadas con contratos.
/// Permite obtener contratos por distintos criterios como ID, usuario,
/// compensaciones asociadas y recibos de pago.
/// </summary>
public class ContractQueryService : IContractQueryService
{
    private readonly IContractRepository _contractRepository;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ContractQueryService"/>.
    /// </summary>
    /// <param name="contractRepository">Repositorio de contratos utilizado para ejecutar consultas.</param>
    public ContractQueryService(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    /// <summary>
    /// Obtiene todos los contratos que se encuentran actualmente activos.
    /// </summary>
    /// <param name="query">Consulta que solicita los contratos activos.</param>
    /// <returns>Colección de contratos activos.</returns>
    public async Task<IEnumerable<Contract>> Handle(GetActiveContractsQuery query)
    {
        return await _contractRepository.GetActiveContractsAsync();
    }

    /// <summary>
    /// Obtiene un contrato específico mediante su identificador único.
    /// </summary>
    /// <param name="query">Consulta que contiene el ID del contrato.</param>
    /// <returns>El contrato solicitado.</returns>
    /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
    public async Task<Contract> Handle(GetContractByIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract;
    }

    /// <summary>
    /// Obtiene todos los contratos asociados a un usuario específico.
    /// </summary>
    /// <param name="query">Consulta que contiene el ID del usuario.</param>
    /// <returns>Lista de contratos del usuario.</returns>
    public async Task<List<Contract?>> Handle(GetContractByUserIdQuery query)
    {
        var contracts = await _contractRepository.GetContractsByUserIdAsync(query.UserId);
        return contracts.Cast<Contract?>().ToList();
    }

    /// <summary>
    /// Obtiene el recibo de pago generado para un contrato específico.
    /// </summary>
    /// <param name="query">Consulta que contiene el ID del contrato.</param>
    /// <returns>El recibo de pago asociado al contrato.</returns>
    /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
    /// <exception cref="InvalidOperationException">Si no existe un recibo emitido para este contrato.</exception>
    public async Task<PaymentReceipt> Handle(GetPaymentReceiptByContractIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract.Receipt 
            ?? throw new InvalidOperationException("No hay recibo emitido para este contrato.");
    }

    /// <summary>
    /// Obtiene todas las compensaciones asociadas a un contrato.
    /// </summary>
    /// <param name="query">Consulta que contiene el ID del contrato.</param>
    /// <returns>Lista de compensaciones del contrato.</returns>
    /// <exception cref="KeyNotFoundException">Si el contrato no existe.</exception>
    public async Task<List<Compensation>> Handle(GetCompensationsByContractIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract.Compensations.ToList();
    }
}
