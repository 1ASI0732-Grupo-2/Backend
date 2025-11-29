using System.Data;
using FluentValidation;
using workstation_backend.Shared.Domain.Repositories;
using workstation_backend.UserContext.Domain;
using workstation_backend.UserContext.Domain.Models.Commands;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Exceptions;
using workstation_backend.UserContext.Domain.Services;

namespace workstation_backend.UserContext.Application.CommandServices;

/// <summary>
/// Servicio encargado de manejar comandos relacionados con usuarios,
/// como creación, actualización y eliminación.
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateUserCommand> validator, IHashService hashService, IJwtEncryptService jwtEncryptService) : IUserCommandService
{
    private readonly IUserRepository _userRepository =
        userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private readonly IValidator<CreateUserCommand> _validator = validator;

    private readonly IHashService _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService));
    private readonly IJwtEncryptService _jwtEncryptService = jwtEncryptService ?? throw new ArgumentNullException(nameof(jwtEncryptService));

    /// <summary>
    /// Crea un nuevo usuario validando datos únicos y rol permitido.
    /// </summary>
    /// <param name="command">Comando con los datos del usuario.</param>
    /// <returns>El usuario creado.</returns>
    /// <exception cref="ArgumentNullException">Si el comando es nulo.</exception>
    /// <exception cref="ValidationException">Si el comando no cumple las reglas de FluentValidation.</exception>
    /// <exception cref="DuplicateNameException">
    /// Si ya existe un usuario con el mismo DNI, email o número telefónico.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">Si el rol del usuario no está entre los valores válidos.</exception>

    public async Task<User> Handle(CreateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var existingUser = await _userRepository.GetByDniAsync(command.Dni);
        if (existingUser is not null)
            throw new DuplicateNameException($"A user with DNI '{command.Dni}' already exists.");

        var userWithSameEmail = await _userRepository.GetByEmailAsync(command.Email);
        if (userWithSameEmail is not null)
            throw new DuplicateNameException($"A user with email '{command.Email}' already exists.");

        var userWithSamePhoneNumber = await _userRepository.GetByPhoneNumberAsync(command.PhoneNumber);
        if (userWithSamePhoneNumber is not null)
            throw new DuplicateNameException($"A user with phone number '{command.PhoneNumber}' already exists.");

        if (!Enum.IsDefined(typeof(UserRole), command.Role))
            throw new ArgumentOutOfRangeException(nameof(command.Role), "Invalid user role value.");

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Dni,
            command.PhoneNumber,
            command.Email,
            command.Role,
            _hashService.HashPassword(command.PasswordHash));

        await _userRepository.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return user;
    }


    /// <summary>
    /// Maneja la eliminación de un usuario del sistema.
    /// </summary>
    /// <param name="command">Comando que contiene el ID del usuario a eliminar.</param>
    /// <exception cref="UserNotFoundException">Si no se encuentra el usuario por el ID.</exception>
    public async Task Handle(DeleteUserCommand command)
    {
        var user = await _userRepository.GetByDniAsync(command.Id.ToString())
                   ?? throw new UserNotFoundException(command.Id);

        _userRepository.Remove(user);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Maneja la actualización de datos de un usuario existente.
    /// </summary>
    /// <param name="command">Comando con los datos actualizados.</param>
    /// <param name="userId">ID del usuario a modificar.</param>
    /// <returns>True si se actualizó correctamente.</returns>
    /// <exception cref="DataException">Si no se encuentra el usuario.</exception>
    /// /// <exception cref="InvalidOperationException">
    /// Si se intenta cambiar el nombre o apellido antes de 1 semana de creación.
    /// </exception>
    /// <exception cref="DuplicateNameException">
    /// Si el nuevo teléfono o correo ya están registrados en otro usuario.
    /// </exception>

    public async Task<bool> Handle(UpdateUserCommand command, Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId)
                ?? throw new DataException("User does not exist.");

        var daysSinceCreated = (DateTime.UtcNow - user.CreatedAt).TotalDays;
        bool isFirstNameChanged = !string.Equals(user.FirstName, command.FirstName, StringComparison.OrdinalIgnoreCase);
        bool isLastNameChanged = !string.Equals(user.LastName, command.LastName, StringComparison.OrdinalIgnoreCase);

        if ((isFirstNameChanged || isLastNameChanged) && daysSinceCreated < 7)
            throw new InvalidOperationException("First name and last name can only be updated after 1 week from user creation.");

        var allUsers = await _userRepository.GetAllAsync();

        bool isPhoneChanged = !string.Equals(user.PhoneNumber, command.PhoneNumber, StringComparison.OrdinalIgnoreCase);
        bool isEmailChanged = !string.Equals(user.Email, command.Email, StringComparison.OrdinalIgnoreCase);

        if (isPhoneChanged && allUsers.Any(u => u.Id != userId && u.PhoneNumber == command.PhoneNumber))
            throw new DuplicateNameException($"The phone number '{command.PhoneNumber}' is already in use.");

        if (isEmailChanged && allUsers.Any(u => u.Id != userId && u.Email == command.Email))
            throw new DuplicateNameException($"The email '{command.Email}' is already in use.");

        user.Update(command.FirstName, command.LastName, command.PhoneNumber, command.Email);

        _userRepository.Update(user);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<string> Handle(LoginCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(command.PasswordHash, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var jwtToken = _jwtEncryptService.Encrypt(user);
        return jwtToken;
    }

}
