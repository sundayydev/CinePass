using CinePass.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinePass.Domain.IRepository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByPhoneAsync(string phoneNumber);
    Task SaveChangesAsync();
}
