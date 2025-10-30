using System;
using Microsoft.AspNetCore.Mvc;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Interface;

[Route("api/workstation/[controller]")]
[ApiController]
[Produces("application/json")]
public class ContractsController(IContractCommandService contractCommandService, IContractQueryService contractQueryService): ControllerBase
{
    private readonly IContractCommandService _contractCommandService = contractCommandService ?? throw new ArgumentNullException(nameof(contractCommandService));
    private readonly IContractQueryService _contractQueryService = contractQueryService ?? throw new ArgumentNullException(nameof(contractQueryService));

}
