using BCrypt.Net;
using CourierManagementSystem.Api.Exceptions;
using CourierManagementSystem.Api.Models.DTOs;
using CourierManagementSystem.Api.Models.Entities;
using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Repositories;

namespace CourierManagementSystem.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(UserRole? role)
    {
        var users = role.HasValue
            ? await _userRepository.GetByRoleAsync(role.Value)
            : await _userRepository.GetAllAsync();

        return users.Select(UserDto.From).ToList();
    }

    public async Task<UserDto> CreateUserAsync(UserRequest request)
    {
        if (request == null)
        {
            throw new ValidationException("Request cannot be null");
        }


        var user = new User
        {
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Role = request.Role,
            CreatedAt = SystemClock.UtcNow
        };

        return UserDto.From(user);
    }

    private async Task<User> GetUserOrThrowAsync(long id)
    {
        return await _userRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);
    }
    var user = await GetUserOrThrowAsync(id);



        if (!string.IsNullOrEmpty(request.Login) && request.Login != user.Login)
        {
            if (await _userRepository.ExistsByLoginAsync(request.Login))
            {
                throw new ValidationException(ErrorMessages.UserWithLoginExists(request.Login));
            }
            user.ChangeLogin(request.Login);
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            user.ChangeName(request.Name);
        }

        if (request.Role.HasValue)
        {
            user.ChangeRole(request.Role.Value);
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
           user.SetPassword(request.Password);

        }

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return UserDto.From(user);
    }

    public async Task DeleteUserAsync(long id)
    {
        private async Task<User> GetUserOrThrowAsync(long id)
        {
            return await _userRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("User", id);
        }
        var user = await GetUserOrThrowAsync(id);


        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}
