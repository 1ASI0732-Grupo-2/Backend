using System;
using workstation_backend.Shared.Domain.Model;

namespace workstation_backend.OfficesContext.Domain.Models.Entities;


/// <summary>
/// Representa un servicio adicional que ofrece una oficina (ej. Wi-Fi, Proyector).
/// </summary>
public class OfficeService : BaseEntity
{
    /// <summary>
    /// Constructor para definir un servicio de oficina.
    /// </summary>
    /// <param name="name">Nombre del servicio.</param>
    /// <param name="description">Descripción detallada del servicio.</param>
    /// <param name="cost">Costo adicional por el servicio.</param>
    public OfficeService(string name, string description, int cost)
    {
        Name = name;
        Description = description;
        IsActive = true;
        Cost = cost;
    }

    /// <summary>
    /// Nombre del servicio (ej. Internet, Estacionamiento).
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Detalles adicionales o condiciones del servicio.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Costo adicional que se cobra por usar este servicio.
    /// </summary>
    public int Cost { get; set; }

    /// <summary>
    /// Identificador de la oficina a la que pertenece este servicio.
    /// </summary>
    public Guid OfficeId { get; set; }

    /// <summary>
    /// Oficina asociada al servicio.
    /// </summary>
    public Office Office { get; set; }
}
