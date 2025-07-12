using System;
using System.Threading.Tasks;

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
        Task<object> GetByEmailAsync(string email);
        
        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<object> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        Task<object> GetByUsernameAsync(string username);
        
        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        Task<bool> CreateAsync(object user);
        
        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        Task<bool> UpdateAsync(object user);
        
        /// <summary>
        /// Elimina un usuario
        /// </summary>
        Task<bool> DeleteAsync(Guid id);
    }
}