using AdminAPI.Data;
using AdminAPI.DTOs.Auth;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public class AuthService(
    AdminDbContext db,
    JwtTokenFactory jwtTokenFactory) : IAuthService
{
    public async Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var password = request.Password;

        var teacher = await db.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Email == username && x.Password == password,
                cancellationToken);

        if (teacher is null)
        {
            return (false, "خطأ في أسم المستخدم أو كلمة المرور", null);
        }

        var token = jwtTokenFactory.CreateToken(teacher, username);
        var redirectPath = teacher.UsersManage ? "/" : "/circles";

        var data = new LoginResponseDto
        {
            Token = token,
            Id = teacher.Id,
            Username = username,
            IsAdmin = teacher.UsersManage,
            IsGirlTeacher = teacher.IsGirlTeacher ?? false,
            IsViewOnly = teacher.IsViewOnly,
            RedirectPath = redirectPath,
        };

        return (true, "OK", data);
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(
        int teacherId,
        ChangePasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (teacherId <= 0)
            return (false, "غير مصرح");

        var teacher = await db.Teachers.FirstOrDefaultAsync(x => x.Id == teacherId, cancellationToken);
        if (teacher is null)
            return (false, "المستخدم غير موجود");

        if (teacher.Password != request.CurrentPassword)
            return (false, "كلمة المرور الحالية غير صحيحة");

        teacher.Password = request.NewPassword.Trim();
        await db.SaveChangesAsync(cancellationToken);

        return (true, "تم تغيير كلمة المرور بنجاح. يرجى تسجيل الدخول مرة أخرى");
    }
}
