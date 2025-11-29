namespace workstation_backend.OfficesContext.Domain.Models.Commands;

/// <summary>
/// Comando para crear una nueva oficina.
/// </summary>
/// <param name="Location">Ubicación de la oficina.</param>
/// <param name="Capacity">Capacidad máxima de personas.</param>
/// <param name="CostPerDay">Costo por día de uso.</param>
/// <param name="Available">Indica si la oficina está disponible.</param>
/// <param name="Services">Lista de servicios ofrecidos.</param>
public record CreateOfficeCommand(string Location, string Description, string ImageUrl, int Capacity, int CostPerDay, bool Available, List<OfficeServiceCommand> Services);

