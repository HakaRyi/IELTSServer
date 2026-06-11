using Auth.Domain.Entities;

namespace Auth.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string email, string username);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
