using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain.Models.Queries;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Application.QueriesServices;

public class ContractQueryService : IContractQueryService
{
    private readonly IContractRepository _contractRepository;

    public ContractQueryService(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<IEnumerable<Contract>> Handle(GetActiveContractsQuery query)
    {
        return await _contractRepository.GetActiveContractsAsync();
    }

    public async Task<Contract> Handle(GetContractByIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract;
    }

    public async Task<List<Contract?>> Handle(GetContractByUserIdQuery query)
    {
        var contracts = await _contractRepository.GetContractsByUserIdAsync(query.UserId);
        return contracts.Cast<Contract?>().ToList();
    }

    public async Task<PaymentReceipt> Handle(GetPaymentReceiptByContractIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract.Receipt 
            ?? throw new InvalidOperationException("No hay recibo emitido para este contrato.");
    }

    public async Task<List<Compensation>> Handle(GetCompensationsByContractIdQuery query)
    {
        var contract = await _contractRepository.GetByIdAsync(query.ContractId)
            ?? throw new KeyNotFoundException($"Contract {query.ContractId} not found.");

        return contract.Compensations.ToList();
    }
}
