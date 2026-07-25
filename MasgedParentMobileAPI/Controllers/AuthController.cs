using System.Security.Claims;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly NewMasgedTeacherAPIDBContext _db;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IWebHostEnvironment _env;
    private readonly StudentRegistrationService _studentRegistrationService;
    private readonly AccountDeletionService _accountDeletionService;

    public AuthController(
        NewMasgedTeacherAPIDBContext db,
        JwtTokenService jwtTokenService,
        IWebHostEnvironment env,
        StudentRegistrationService studentRegistrationService,
        AccountDeletionService accountDeletionService)
    {
        _db = db;
        _jwtTokenService = jwtTokenService;
        _env = env;
        _studentRegistrationService = studentRegistrationService;
        _accountDeletionService = accountDeletionService;
    }

    /// <summary>Login for parents who already activated their password.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FatherPhone) || string.IsNullOrWhiteSpace(request.Password))
            return Unauthorized(new { message = "رقم الجوال أو كلمة المرور غير صحيحة" });

        var variants = PhoneNormalizer.GetVariants(request.FatherPhone).ToList();
        var canonicalPhone = PhoneNormalizer.ToCanonical(request.FatherPhone);

        var parent = await _db.RegisterForms
            .Where(r => r.ThePassword == request.Password &&
                        (variants.Contains(r.FatherPhone) ||
                         variants.Contains(r.FatherPhone2)))
            .FirstOrDefaultAsync();

        if (parent == null)
            return Unauthorized(new { message = "رقم الجوال أو كلمة المرور غير صحيحة" });

        var token = _jwtTokenService.GenerateToken(
            parent.Id,
            canonicalPhone,
            parent.FatherName ?? string.Empty);

        return Ok(new LoginResponse
        {
            Token = token,
            ParentId = parent.Id,
            FatherName = parent.FatherName ?? string.Empty,
            Phone = canonicalPhone,
        });
    }

    /// <summary>
    /// Start registration: parent must exist on at least one student row with unset password.
    /// Sends (stores) OTP; integrate SMS/WhatsApp externally in production — dev returns debugOtp.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FatherName) ||
            string.IsNullOrWhiteSpace(request.FatherPhone) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "يرجى تعبئة جميع الحقول" });

        if (request.Password.Length < 6)
            return BadRequest(new { message = "كلمة المرور يجب أن تكون 6 أحرف على الأقل" });

        var canonical = PhoneNormalizer.ToCanonical(request.FatherPhone);
        var variants = PhoneNormalizer.GetVariants(request.FatherPhone).ToList();

        var rows = await _db.RegisterForms
            .Where(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .ToListAsync();

        if (rows.Count == 0)
            return BadRequest(new { message = "لم يتم العثور على بياناتكم، يرجى التواصل مع الإدارة لإضافة ابنكم أولًا" });

        if (rows.Any(r => !string.IsNullOrWhiteSpace(r.ThePassword)))
            return Conflict(new { message = "تم التسجيل مسبقًا، يرجى تسجيل الدخول" });

        var otp = Random.Shared.Next(100000, 999999).ToString();

        var existing = await _db.ParentRegistrationOtps.FindAsync(new object[] { canonical });
        if (existing != null)
            _db.ParentRegistrationOtps.Remove(existing);

        _db.ParentRegistrationOtps.Add(new ParentRegistrationOtp
        {
            CanonicalPhone = canonical,
            FatherName = request.FatherName.Trim(),
            PasswordPlain = request.Password,
            OtpCode = otp,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(15),
        });

        await _db.SaveChangesAsync();

        object payload = _env.IsDevelopment()
            ? new { message = "تم إنشاء رمز التحقق (وضع التطوير)", debugOtp = otp }
            : new { message = "تم إرسال رمز التحقق" };

        return Ok(payload);
    }

    /// <summary>
    /// Student enrollment (public form fields + password). Creates RegisterForm and returns JWT.
    /// </summary>
    [HttpPost("student-register")]
    public async Task<ActionResult<StudentRegistrationResponseDto>> StudentRegister(
        [FromBody] StudentRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _studentRegistrationService.RegisterAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            var message = ex.Message;
            if (message.Contains("مسبقًا"))
                return Conflict(new { message });

            return BadRequest(new { message });
        }
    }

    /// <summary>Completes registration after OTP; issues JWT identical to login.</summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<LoginResponse>> VerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FatherPhone) || string.IsNullOrWhiteSpace(request.Otp))
            return BadRequest(new { message = "بيانات غير صالحة" });

        var otp = request.Otp.Trim();
        if (otp.Length != 6 || !otp.All(char.IsDigit))
            return BadRequest(new { message = "رمز التحقق غير صالح" });

        var canonical = PhoneNormalizer.ToCanonical(request.FatherPhone);
        var challenge = await _db.ParentRegistrationOtps.FirstOrDefaultAsync(x => x.CanonicalPhone == canonical);
        if (challenge == null)
            return BadRequest(new { message = "لم يتم طلب التحقق لهذا الرقم" });

        if (challenge.ExpiresUtc < DateTime.UtcNow)
        {
            _db.ParentRegistrationOtps.Remove(challenge);
            await _db.SaveChangesAsync();
            return BadRequest(new { message = "انتهت صلاحية رمز التحقق، أعد المحاولة" });
        }

        if (challenge.OtpCode != otp)
            return BadRequest(new { message = "رمز التحقق غير صحيح" });

        var variants = PhoneNormalizer.GetVariants(request.FatherPhone).ToList();

        var students = await _db.RegisterForms
            .Where(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2))
            .ToListAsync();

        if (students.Count == 0)
            return BadRequest(new { message = "تعذر إكمال التسجيل" });

        foreach (var s in students)
        {
            s.ThePassword = challenge.PasswordPlain;
            if (!string.IsNullOrWhiteSpace(challenge.FatherName))
                s.FatherName = challenge.FatherName;
        }

        _db.ParentRegistrationOtps.Remove(challenge);
        await _db.SaveChangesAsync();

        var parentRow = students[0];

        var token = _jwtTokenService.GenerateToken(
            parentRow.Id,
            canonical,
            parentRow.FatherName ?? string.Empty);

        return Ok(new LoginResponse
        {
            Token = token,
            ParentId = parentRow.Id,
            FatherName = parentRow.FatherName ?? string.Empty,
            Phone = canonical,
        });
    }

    /// <summary>Permanently deletes the authenticated parent account after password confirmation.</summary>
    [Authorize]
    [HttpPost("delete-account")]
    public async Task<IActionResult> DeleteAccount(
        [FromBody] DeleteAccountRequestDto request,
        CancellationToken cancellationToken)
    {
        var fatherPhone = User.FindFirstValue("fatherPhone");
        if (string.IsNullOrWhiteSpace(fatherPhone))
            return Unauthorized(new { message = "غير مصرح" });

        var (success, message) = await _accountDeletionService.DeleteParentAccountAsync(
            fatherPhone,
            request.Password ?? string.Empty,
            cancellationToken);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }
}
