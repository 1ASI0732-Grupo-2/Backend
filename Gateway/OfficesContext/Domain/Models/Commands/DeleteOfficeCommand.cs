namespace workstation_backend.OfficesContext.Domain.Models.Commands;

/// <summary>
/// Comando para eliminar una oficina existente.
/// </summary>
/// <param name="Id">Identificador único de la oficina.</param>
public record class DeleteOfficeCommand(Guid Id);

