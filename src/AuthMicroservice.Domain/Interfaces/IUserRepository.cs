using System;
using System.Threading.Tasks;
using AuthMicroservice.Domain.Entities;

namespace AuthMicroservice.Domain.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones con usuarios
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        Task<User> GetByEmailAsync(string email);
        
        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        Task<User> GetByUsernameAsync(string username);
        
        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task<bool> CreateAsync(User user);
        
        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        Task<bool> UpdateAsync(User user);
        
        /// <summary>
        /// Elimina un usuario
        /// </summary>
        Task<bool> DeleteAsync(Guid id);
    }
}