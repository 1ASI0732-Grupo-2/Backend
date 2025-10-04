using workstation_backend.OfficesContext.Domain.Models.Entities;

namespace workstation_backend.OfficesContext.Domain.Models.Commands;

/// <summary>
/// Comando para registrar un servicio asociado a una oficina.
/// </summary>
/// <param name="Name">Nombre del servicio (ej. Wi-Fi, Proyector).</param>
/// <param name="Description">Descripción del servicio ofrecido.</param>
/// <param name="Cost">Costo adicional por el servicio.</param>
public record OfficeServiceCommand(string Name, string Description, int Cost);
