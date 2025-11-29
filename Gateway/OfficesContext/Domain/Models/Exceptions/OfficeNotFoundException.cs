using System;

namespace workstation_backend.OfficesContext.Domain.Models.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se encuentra una oficina.
/// </summary>
public class OfficeNotFoundException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia con un mensaje predeterminado.
    /// </summary>
    public OfficeNotFoundException() 
        : base("Not offices found") { }

    /// <summary>
    /// Inicializa una nueva instancia con un mensaje personalizado.
    /// </summary>
    /// <param name="message">Mensaje que describe el error.</param>
    public OfficeNotFoundException(string message) 
        : base(message) { }
}

