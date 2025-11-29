using ContractsContext.Domain.Models.Entities;
using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Queries;

public record GetActiveContractsQuery : IRequest<List<Contract?>>;