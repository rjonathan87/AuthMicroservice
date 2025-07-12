using AuthMicroservice.Application.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthMicroservice.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de hash y verificación de contraseñas.
    /// </summary>
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        /// <summary>
        /// Genera un hash seguro para una contraseña.
        /// </summary>
        /// <param name="password">La contraseña en texto plano a hashear.</param>
        /// <returns>El hash de la contraseña.</returns>
        public string HashPassword(string password)
        {
            // Generar un salt aleatorio
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derivar el hash usando PBKDF2
            byte[] hash = GetHash(password, salt);

            // Combinar salt y hash en un string base64
            byte[] combinedBytes = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, combinedBytes, SaltSize, HashSize);
            
            return Convert.ToBase64String(combinedBytes);
        }

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash.
        /// </summary>
        /// <param name="password">La contraseña en texto plano a verificar.</param>
        /// <param name="passwordHash">El hash almacenado de la contraseña.</param>
        /// <returns>True si la contraseña coincide con el hash, false en caso contrario.</returns>
        public bool VerifyPassword(string password, string passwordHash)
        {
            // Convertir el hash almacenado de base64 a bytes
            byte[] combinedBytes;
            try
            {
                combinedBytes = Convert.FromBase64String(passwordHash);
            }
            catch (FormatException)
            {
                // El hash no es un string base64 válido
                return false;
            }

            // Verificar que el hash tenga el tamaño correcto
            if (combinedBytes.Length != SaltSize + HashSize)
            {
                return false;
            }

            // Extraer el salt y el hash original
            byte[] salt = new byte[SaltSize];
            byte[] originalHash = new byte[HashSize];
            Buffer.BlockCopy(combinedBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(combinedBytes, SaltSize, originalHash, 0, HashSize);

            // Calcular el hash de la contraseña proporcionada
            byte[] newHash = GetHash(password, salt);

            // Comparar los hashes
            return CompareByteArrays(originalHash, newHash);
        }

        private byte[] GetHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            // Usar una comparación de tiempo constante para evitar ataques de tiempo
            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}