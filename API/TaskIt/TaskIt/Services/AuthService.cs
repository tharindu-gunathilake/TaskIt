using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskIt.Data;
using TaskIt.Domain;
using TaskIt.Interfaces;
using TaskIt.Models.Requests;
using TaskIt.Models.Responses;
using TaskIt.Models.Shared;

namespace TaskIt.Services
{
    public class AuthService : IAuthService
    {
        private readonly TaskItDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(TaskItDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<BaseResponse<UserDto>> AuthenticateUserAsync(AuthenticateUserDto userCredentials)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email == userCredentials.Email &&
                        u.DeletedAt == null);

                if (user == null)
                {
                    return BaseResponse<UserDto>.Fail(
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    );
                }

                if (String.IsNullOrEmpty(userCredentials.Password))
                {
                    return BaseResponse<UserDto>.Fail(
                        "Password cannot be empty",
                        StatusCodes.Status400BadRequest
                    );
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    userCredentials.Password
                );

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return BaseResponse<UserDto>.Fail(
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    );
                }

                return BaseResponse<UserDto>.Ok(
                    new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    },
                    "User authenticated successfully"
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"AuthenticateUserAsync Error : {e.Message}");
                return BaseResponse<UserDto>.Fail(
                    "Failed to authenticate user",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<BaseResponse<UserDto>> RegisterUserAsync(RegisterUserDto newUser)
        {
            try
            {
                var existingUser = _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Email == newUser.Email || u.UserName == newUser.UserName);
                if (existingUser != null)
                {
                    return BaseResponse<UserDto>.Fail("User with the same email or username alredy exists", StatusCodes.Status400BadRequest);
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    PasswordHash = string.Empty,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, newUser.Password);

                _dbContext.Users.Add(user);
                var result = await _dbContext.SaveChangesAsync();

                return result > 0
                    ? BaseResponse<UserDto>.Ok(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    }, "User registered successfully")
                    : BaseResponse<UserDto>.Fail("Failed to register user", StatusCodes.Status500InternalServerError);

            }
            catch (Exception e)
            {
                Console.WriteLine($"UpdateTaskAsync Error : {e.Message}");
                return BaseResponse<UserDto>.Fail("Failed to Register User", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
