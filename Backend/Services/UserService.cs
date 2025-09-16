using GameShelf.API.Backend.Data;
using GameShelf.API.Backend.DTOs;
using GameShelf.API.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShelf.API.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _db;
        
        public UserService (ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> DoesUserExistAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User> RegisterUserAsync(RegisterDto registerDto)
        {
            var passwordhash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordhash
            };
            _db.Users.Add(user);
             await _db.SaveChangesAsync();

            return user;
        }
    }
}
