using System;
using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.UserContext.Domain.Services;

public interface IJwtEncryptService
{
    string Encrypt(User user);
    User Decrypt(string encrypted);
}
