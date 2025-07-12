namespace AuthMicroservice.Application.Services
{
    using AuthMicroservice.Application.DTOs;
    using AuthMicroservice.Application.Interfaces;
    using AuthMicroservice.Domain.Entities;
    using AuthMicroservice.Domain.Exceptions;
    using AuthMicroservice.Domain.Interfaces;

    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserDto> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            return user == null
                ? throw new NotFoundException("User not found")
                : new UserDto
            {
                UserId = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                IsLocked = user.IsLocked
            };
        }
    }
}