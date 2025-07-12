using AuthMicroservice.Application.DTOs;

namespace AuthMicroservice.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserProfileAsync(string userId);
    }
}