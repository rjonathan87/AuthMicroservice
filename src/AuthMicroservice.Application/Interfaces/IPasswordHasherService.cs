namespace AuthMicroservice.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de hash y verificación de contraseñas.
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Genera un hash seguro para una contraseña.
        /// </summary>
        /// <param name="password">La contraseña en texto plano a hashear.</param>
        /// <returns>El hash de la contraseña.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash.
        /// </summary>
        /// <param name="password">La contraseña en texto plano a verificar.</param>
        /// <param name="passwordHash">El hash almacenado de la contraseña.</param>
        /// <returns>True si la contraseña coincide con el hash, false en caso contrario.</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}