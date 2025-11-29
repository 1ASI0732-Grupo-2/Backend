using System;
using Org.BouncyCastle.Crypto.Generators;
using workstation_backend.UserContext.Domain.Services;

namespace workstation_backend.UserContext.Application.HashServices;

public class HashService : IHashService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

}
