using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.UserContext.Interfaces.REST.Resources;
using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.UserContext.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        return new UserResource(user.Id, user.FirstName, user.LastName, user.Dni, user.PhoneNumber, user.Email,
            user.Role, user.PasswordHash);
    }
}