using System;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.OfficesContext.Domain.Models.Entities;

namespace workstation_backend.OfficesContext.Domain.Services;

/// <summary>
/// Define las operaciones de comando (crear, actualizar, eliminar) para oficinas.
/// </summary>
public interface IOfficeCommandService
{
    /// <summary>
    /// Crea una nueva oficina en el sistema.
    /// </summary>
    /// <param name="command">Datos necesarios para crear la oficina.</param>
    /// <returns>La oficina creada.</returns>
    Task<Office> Handle(CreateOfficeCommand command);

    /// <summary>
    /// Elimina una oficina existente.
    /// </summary>
    /// <param name="command">Comando que contiene el identificador de la oficina a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, false si no.</returns>
    Task<bool> Handle(DeleteOfficeCommand command);

    /// <summary>
    /// Actualiza los datos de una oficina existente.
    /// </summary>
    /// <param name="command">Datos actualizados de la oficina.</param>
    /// <param name="Id">Identificador de la oficina que se desea actualizar.</param>
    /// <returns>True si la actualización fue exitosa, false si no.</returns>
    Task<bool> Handle(UpdateOfficeCommand command, Guid Id);
}
