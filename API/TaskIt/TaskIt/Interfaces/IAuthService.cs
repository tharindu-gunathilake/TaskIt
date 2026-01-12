using TaskIt.Models.Requests;
using TaskIt.Models.Responses;
using TaskIt.Models.Shared;

namespace TaskIt.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<UserDto>> RegisterUserAsync(RegisterUserDto newUser);
        Task<BaseResponse<UserDto>> AuthenticateUserAsync(AuthenticateUserDto userCredentials);
    }
}
