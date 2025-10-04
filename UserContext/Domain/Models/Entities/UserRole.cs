namespace workstation_backend.UserContext.Domain.Models.Entities;

/// <summary>
/// Define los roles posibles que puede tener un usuario.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Usuario que busca oficinas para alquilar.
    /// </summary>
    Seeker = 1,

    /// <summary>
    /// Usuario que ofrece oficinas en alquiler.
    /// </summary>
    Lessor = 2
}
