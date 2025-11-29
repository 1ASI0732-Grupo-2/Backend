using System;

namespace workstation_backend.OfficesContext.Domain.Models.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se encuentran servicios disponibles o asociados.
/// </summary>
public class NotServicesFoundException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia con un mensaje predeterminado.
    /// </summary>
    public NotServicesFoundException() 
        : base("Not services found") { }

    /// <summary>
    /// Inicializa una nueva instancia con un mensaje personalizado.
    /// </summary>
    /// <param name="message">Mensaje que describe el error.</param>
    public NotServicesFoundException(string message) 
        : base(message) { }

    /// <summary>
    /// Inicializa una nueva instancia con un mensaje y una excepción interna.
    /// </summary>
    /// <param name="message">Mensaje que describe el error.</param>
    /// <param name="inner">Excepción interna que causó este error.</param>
    public NotServicesFoundException(string message, Exception inner) 
        : base(message, inner) { }
}

