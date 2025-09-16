using GameShelf.API.Backend.DTOs;
using GameShelf.API.Backend.Models;

namespace GameShelf.API.Services
{
    public interface IUserService
    {
        Task<bool> DoesUserExistAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> RegisterUserAsync(RegisterDto registerDto);

    }
}
