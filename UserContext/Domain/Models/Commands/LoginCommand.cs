namespace workstation_backend.UserContext.Domain.Models.Commands;

public record class LoginCommand(string Email, String PasswordHash);
