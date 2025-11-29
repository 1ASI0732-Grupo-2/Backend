using System;

namespace workstation_backend.UserContext.Domain.Services;

public interface IHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
