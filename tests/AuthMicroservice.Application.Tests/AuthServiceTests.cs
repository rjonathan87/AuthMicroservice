using Xunit;
using AuthMicroservice.Infrastructure.Services;

namespace AuthMicroservice.Application.Tests
{
    public class PasswordHasherServiceTests
    {
        [Fact]
        public void HashPassword_ShouldGenerateConsistentHash()
        {
            // Arrange
            var passwordHasher = new PasswordHasherService();
            var password = "TestPassword123";

            // Act
            var hash1 = passwordHasher.HashPassword(password);
            var hash2 = passwordHasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // Hashes should be unique due to random salt
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrueForValidPassword()
        {
            // Arrange
            var passwordHasher = new PasswordHasherService();
            var password = "TestPassword123";
            var hash = passwordHasher.HashPassword(password);

            // Act
            var isValid = passwordHasher.VerifyPassword(password, hash);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForInvalidPassword()
        {
            // Arrange
            var passwordHasher = new PasswordHasherService();
            var password = "TestPassword123";
            var hash = passwordHasher.HashPassword(password);

            // Act
            var isValid = passwordHasher.VerifyPassword("WrongPassword", hash);

            // Assert
            Assert.False(isValid);
        }
    }
}