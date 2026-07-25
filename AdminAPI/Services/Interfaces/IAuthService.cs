using AdminAPI.DTOs.Auth;

namespace AdminAPI.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> ChangePasswordAsync(
        int teacherId,
        ChangePasswordRequestDto request,
        CancellationToken cancellationToken = default);
}
