namespace workstation_backend.UserContext.Domain.Models.Exceptions;

/// <summary>
/// Excepción lanzada cuando no se encuentra un usuario.
/// </summary>
public class UserNotFoundException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia indicando el ID del usuario no encontrado.
    /// </summary>
    /// <param name="userId">ID del usuario que no fue encontrado.</param>
    public UserNotFoundException(Guid userId)
        : base($"User with ID {userId} was not found.") { }

    /// <summary>
    /// Inicializa una nueva instancia con un mensaje personalizado.
    /// </summary>
    /// <param name="message">Mensaje que describe el error.</param>
    public UserNotFoundException(string message) 
        : base(message) { }

    /// <summary>
    /// Inicializa una nueva instancia con un mensaje y una excepción interna.
    /// </summary>
    /// <param name="message">Mensaje que describe el error.</param>
    /// <param name="innerException">Excepción interna que causó este error.</param>
    public UserNotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}
