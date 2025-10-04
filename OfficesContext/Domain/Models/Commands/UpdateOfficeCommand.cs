namespace workstation_backend.OfficesContext.Domain.Models.Commands;

/// <summary>
/// Comando para actualizar los datos de una oficina existente.
/// </summary>
/// <param name="Id">Identificador de la oficina.</param>
/// <param name="Location">Nueva ubicación de la oficina.</param>
/// <param name="Capacity">Nueva capacidad máxima.</param>
/// <param name="CostPerDay">Nuevo costo por día.</param>
/// <param name="Available">Nueva disponibilidad.</param>
public record UpdateOfficeCommand(Guid Id, string Location, string Description, string ImageUrl, int Capacity, int CostPerDay, bool Available);

