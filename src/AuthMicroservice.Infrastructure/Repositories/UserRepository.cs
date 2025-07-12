using AuthMicroservice.Domain.Interfaces;
using AuthMicroservice.Domain.Exceptions; // Added directive for NotFoundException
using AuthMicroservice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AuthMicroservice.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// </summary>
    public class UserRepository(AuthDbContext context) : IUserRepository
    {
        private readonly AuthDbContext _context = context;

        public async Task<Domain.Entities.User?> GetByEmailAsync(string email)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (userEntity == null)
            {
                return null;
            }

            // Mapear la entidad de infraestructura a la entidad de dominio
            return new Domain.Entities.User
            {
                Id = userEntity.UserId,
                Username = userEntity.Username,
                Email = userEntity.Email,
                IsLocked = userEntity.IsLocked,
                PasswordHash = userEntity.PasswordHash // Agregar PasswordHash
            };
        }

        public async Task<Domain.Entities.User?> GetByIdAsync(Guid id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                throw new NotFoundException($"User with ID {id} not found.");
            }

            // Map the infrastructure entity to the domain entity
            return new Domain.Entities.User
            {
                Id = userEntity.UserId,
                Username = userEntity.Username,
                Email = userEntity.Email,
                IsLocked = userEntity.IsLocked,
                PasswordHash = userEntity.PasswordHash
            };
        }

        public async Task<object?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> CreateAsync(object user)
        {
            if (user is AuthMicroservice.Infrastructure.Data.User userEntity)
            {
                _context.Users.Add(userEntity);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(object user)
        {
            if (user is AuthMicroservice.Infrastructure.Data.User userEntity)
            {
                _context.Entry(userEntity).State = EntityState.Modified;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        Task<Domain.Entities.User> IUserRepository.GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAsync(Domain.Entities.User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Domain.Entities.User user)
        {
            throw new NotImplementedException();
        }
    }
}