using System;
using Microsoft.AspNetCore.Mvc;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Queries;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.ContractsContext.Interface.Resources;
using workstation_backend.ContractsContext.Interface.Transform;

namespace workstation_backend.ContractsContext.Interface;

[Route("api/workstation/[controller]")]
[ApiController]
[Produces("application/json")]
public class ContractsController(IContractCommandService contractCommandService, IContractQueryService contractQueryService) : ControllerBase
{
    private readonly IContractCommandService _contractCommandService = contractCommandService ?? throw new ArgumentNullException(nameof(contractCommandService));
    private readonly IContractQueryService _contractQueryService = contractQueryService ?? throw new ArgumentNullException(nameof(contractQueryService));

    [HttpPost]
    public async Task<ActionResult<ContractResource>> CreateContract([FromBody] CreateContractResource resource)
    {
        
        var command = CreateContractCommandAssembler.ToCommand(resource);
        var contract = await _contractCommandService.Handle(command);
        var contractResource = ContractResourceAssembler.ToResource(contract);
        return CreatedAtAction(nameof(GetContractById), new { id = contract.Id }, contractResource);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContractResource>> GetContractById(Guid id)
    {
        var query = new GetContractByIdQuery(id);
        var contract = await _contractQueryService.Handle(query);
        var resource = ContractResourceAssembler.ToResource(contract);
        return Ok(resource);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ContractResource>>> GetContractsByUserId(Guid userId)
    {
        var query = new GetContractByUserIdQuery(userId);
        var contracts = await _contractQueryService.Handle(query);
        var resources = contracts.Select(ContractResourceAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ContractResource>>> GetActiveContracts()
    {
        var query = new GetActiveContractsQuery();
        var contracts = await _contractQueryService.Handle(query);
        var resources = contracts.Select(ContractResourceAssembler.ToResource);
        return Ok(resources);
    }

    [HttpPost("{contractId}/clauses")]
    public async Task<ActionResult<ClauseResource>> AddClause(Guid contractId, [FromBody] AddClauseResource resource)
    {
        var command = AddClauseCommandAssembler.ToCommand(contractId, resource);
        var clause = await _contractCommandService.Handle(command);
        var clauseResource = ClauseResourceAssembler.ToResource(clause);
        return CreatedAtAction(nameof(GetContractById), new { id = contractId }, clauseResource);
    }

    [HttpPost("{contractId}/signatures")]
    public async Task<ActionResult<ContractResource>> SignContract(Guid contractId, [FromBody] SignContractResource resource)
    {
        var command = SignContractCommandAssembler.ToCommand(contractId, resource);
        var contract = await _contractCommandService.Handle(command);
        var contractResource = ContractResourceAssembler.ToResource(contract);
        return Ok(contractResource);
    }

    [HttpPost("{contractId}/activate")]
    public async Task<ActionResult> ActivateContract(Guid contractId)
    {
        var command = new ActivateContractCommand(contractId);
        await _contractCommandService.Handle(command);
        return Ok(new { message = "Contrato activado exitosamente" });
    }

    [HttpPost("{contractId}/compensations")]
    public async Task<ActionResult<CompensationResource>> AddCompensation(Guid contractId, [FromBody] AddCompensationResource resource)
    {
        var command = AddCompensationCommandAssembler.ToCommand(contractId, resource);
        var compensation = await _contractCommandService.Handle(command);
        var compensationResource = CompensationResourceAssembler.ToResource(compensation);
        return CreatedAtAction(nameof(GetCompensationsByContractId), new { contractId }, compensationResource);
    }

    [HttpGet("{contractId}/compensations")]
    public async Task<ActionResult<IEnumerable<CompensationResource>>> GetCompensationsByContractId(Guid contractId)
    {
        var query = new GetCompensationsByContractIdQuery(contractId);
        var compensations = await _contractQueryService.Handle(query);
        var resources = compensations.Select(CompensationResourceAssembler.ToResource);
        return Ok(resources);
    }

    [HttpGet("{contractId}/receipt")]
    public async Task<ActionResult<PaymentReceiptResource>> GetPaymentReceipt(Guid contractId)
    {
        var query = new GetPaymentReceiptByContractIdQuery(contractId);
        var receipt = await _contractQueryService.Handle(query);
        var resource = PaymentReceiptResourceAssembler.ToResource(receipt);
        return Ok(resource);
    }

    [HttpPut("{contractId}/receipt")]
    public async Task<ActionResult<PaymentReceiptResource>> UpdateReceipt(Guid contractId, [FromBody] UpdateReceiptResource resource)
    {
        var command = UpdateReceiptCommandAssembler.ToCommand(contractId, resource);
        var receipt = await _contractCommandService.Handle(command);
        var receiptResource = PaymentReceiptResourceAssembler.ToResource(receipt);
        return Ok(receiptResource);
    }

    [HttpPost("{contractId}/finish")]
    public async Task<ActionResult> FinishContract(Guid contractId, [FromBody] FinishContractResource resource)
    {
        var command = new FinishContractCommand(contractId, resource.Reason);
        await _contractCommandService.Handle(command);
        return Ok(new { message = "Contrato finalizado exitosamente" });
    }


}
