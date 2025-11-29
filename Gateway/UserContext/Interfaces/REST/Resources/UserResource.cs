using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.UserContext.Interfaces.REST.Resources;

public record class UserResource(Guid Id, 
    string firstName, 
    string lastName, 
    string dni,
    string phoneNumber, 
    string email,
    UserRole role,
    String password);