using System.Security.Claims;
using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly NewMasgedTeacherAPIDBContext _db;
    private readonly StudentService _studentService;
    private readonly MemorizingArchiveService _archiveService;
    private readonly StudentPhotoUploadOptions _photoUploadOptions;

    public StudentsController(
        NewMasgedTeacherAPIDBContext db,
        StudentService studentService,
        MemorizingArchiveService archiveService,
        IOptions<StudentPhotoUploadOptions> photoUploadOptions)
    {
        _db = db;
        _studentService = studentService;
        _archiveService = archiveService;
        _photoUploadOptions = photoUploadOptions.Value;
    }

    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAll()
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var students = await _studentService.GetParentStudentsAsync(fatherPhone);
        if (students.Count == 0) return Ok(new List<StudentDto>());

        var today = DateTime.Today;
        var weekStart = AttendanceHelper.GetWeekStartSaturday(today);
        var weekEnd = weekStart.AddDays(6);
        var studentIds = students.Select(s => s.Id).ToList();

        var weekAttendances = await _db.CircleAttendances
            .Where(a => studentIds.Contains(a.StudentId) &&
                        a.AttendanceDateTime.Date >= weekStart &&
                        a.AttendanceDateTime.Date <= weekEnd)
            .ToListAsync();

        var todayDepartures = await _db.CircleDepartures
            .Where(d => studentIds.Contains(d.StudentId) && d.DepartureDate == DateOnly.FromDateTime(today))
            .ToListAsync();

        var result = new List<StudentDto>();
        foreach (var student in students)
        {
            var attendances = weekAttendances.Where(a => a.StudentId == student.Id).ToList();
            var departure = todayDepartures.FirstOrDefault(d => d.StudentId == student.Id);
            result.Add(await _studentService.BuildListItemAsync(student, attendances, departure, weekStart));
        }

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentProfileDto>> GetById(int id)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        var today = DateTime.Today;
        var weekStart = AttendanceHelper.GetWeekStartSaturday(today);
        var weekEnd = weekStart.AddDays(6);

        var weekAttendances = await _db.CircleAttendances
            .Where(a => a.StudentId == id &&
                        a.AttendanceDateTime.Date >= weekStart &&
                        a.AttendanceDateTime.Date <= weekEnd)
            .ToListAsync();

        var todayDeparture = await _db.CircleDepartures
            .FirstOrDefaultAsync(d => d.StudentId == id && d.DepartureDate == DateOnly.FromDateTime(today));

        var profile = await _studentService.BuildProfileAsync(student, weekAttendances, todayDeparture, weekStart);
        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<StudentProfileDto>> Create([FromBody] AddStudentRequest request)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new { message = "يرجى إدخال الاسم الرباعي" });

        var variants = PhoneNormalizer.GetVariants(fatherPhone).ToList();
        var parentRef = await _db.RegisterForms
            .FirstOrDefaultAsync(r => variants.Contains(r.FatherPhone) || variants.Contains(r.FatherPhone2));

        var fatherName = User.FindFirstValue("fatherName")
            ?? request.ParentName
            ?? parentRef?.FatherName
            ?? string.Empty;

        var student = new RegisterForm
        {
            StudentName = request.FullName.Trim(),
            FullName = request.FullName.Trim(),
            FatherName = fatherName,
            FatherPhone = fatherPhone,
            FatherPhone2 = parentRef?.FatherPhone2 ?? string.Empty,
            StudentPhone = request.Phone ?? fatherPhone,
            StudentGender = parentRef?.StudentGender ?? "ذكر",
            ThePassword = parentRef?.ThePassword ?? string.Empty,
            Birthdate = request.BirthDate,
            Age = StudentService.CalculateAge(request.BirthDate),
            CreatedAt = DateTime.Now,
            IsSpecial = false,
            IsElite = false,
        };

        _db.RegisterForms.Add(student);
        await _db.SaveChangesAsync();

        var followup = new ParentFollowup
        {
            StudentId = student.Id,
            Address = request.Address ?? string.Empty,
            MaritalStatus = request.MaritalStatus ?? string.Empty,
            HealthCondition = request.HasHealthCondition ? "نعم" : "لا",
            HealthDetails = request.HealthDetails ?? string.Empty,
            LearningDifficulties = request.HasLearningDifficulties ? "نعم" : "لا",
            LearningDifficultiesNotes = request.LearningDifficultiesDetails ?? string.Empty,
            PhotoPath = string.Empty,
        };

        _db.ParentFollowups.Add(followup);
        await _db.SaveChangesAsync();

        student.ParentFollowup = followup;
        var profile = await _studentService.BuildProfileAsync(student, new List<CircleAttendance>(), null, AttendanceHelper.GetWeekStartSaturday(DateTime.Today));
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, profile);
    }

    /// <summary>Current plan overview for parent (read-only).</summary>
    [HttpGet("{id:int}/plan-overview")]
    public async Task<ActionResult<ParentStudentPlanOverviewDto>> GetPlanOverview(int id)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        var overview = await _studentService.GetPlanOverviewAsync(student);
        return Ok(overview);
    }

    /// <summary>Paginated memorizing archive (حفظ / مراجعة records).</summary>
    [HttpGet("{id:int}/memorizing-archive")]
    public async Task<ActionResult<PagedResultDto<MemorizingArchiveItemDto>>> GetMemorizingArchive(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? surahSearch = null,
        [FromQuery] string? typeFilter = null,
        CancellationToken cancellationToken = default)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        var rows = await _archiveService.GetForParentAsync(
            _db,
            id,
            surahSearch,
            typeFilter,
            page,
            pageSize,
            cancellationToken);

        return Ok(rows);
    }

    /// <summary>Paginated plan rows for parent (read-only).</summary>
    [HttpGet("{id:int}/plan-rows")]
    public async Task<ActionResult<PagedResultDto<ParentPlanRowDto>>> GetPlanRows(
        int id,
        [FromQuery] string planType = "حفظ",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        var plan = await _studentService.GetCurrentPlanAsync(id);
        if (plan is null)
        {
            return Ok(new PagedResultDto<ParentPlanRowDto>
            {
                Page = Math.Max(1, page),
                PageSize = Math.Clamp(pageSize, 1, 50),
            });
        }

        var rows = await _studentService.GetPlanRowsAsync(id, plan.Id, planType, page, pageSize);
        return Ok(rows);
    }

    /// <summary>Current memorize/revise assignment derived from teacher student plans.</summary>
    [HttpGet("{id:int}/quran-assignment")]
    public async Task<ActionResult<StudentQuranAssignmentDto>> GetQuranAssignment(int id)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        var memorizing = await _db.StudentPlanMemorizings
            .Include(m => m.Surah)
            .Where(m => m.StudentId == id)
            .OrderByDescending(m => m.PlanDate)
            .FirstOrDefaultAsync();

        var revise = await _db.StudentPlanRevises
            .Include(r => r.Surah)
            .Where(r => r.StudentId == id)
            .OrderByDescending(r => r.PlanDate)
            .FirstOrDefaultAsync();

        static int ClampAyah(int n) => n < 1 ? 1 : n;

        var dto = new StudentQuranAssignmentDto
        {
            MemorizeSurahId = memorizing?.SurahId ?? 1,
            MemorizeSurahNameArabic = memorizing?.Surah?.NameAr ?? string.Empty,
            MemorizeFromAyah = memorizing != null ? ClampAyah(memorizing.FromAyahNumber) : 1,
            MemorizeToAyah = memorizing != null ? ClampAyah(memorizing.ToAyahNumber) : 1,
            ReviseSurahId = revise?.SurahId,
            ReviseSurahNameArabic = revise?.Surah?.NameAr,
            ReviseFromAyah = revise != null ? ClampAyah(revise.FromAyahNumber) : 0,
            ReviseToAyah = revise != null ? ClampAyah(revise.ToAyahNumber) : 0,
        };

        return Ok(dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentProfileDto>> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            student.FullName = request.FullName.Trim();
            student.StudentName = request.FullName.Trim();
        }

        if (request.BirthDate.HasValue)
        {
            student.Birthdate = request.BirthDate;
            student.Age = StudentService.CalculateAge(request.BirthDate);
        }

        if (!string.IsNullOrWhiteSpace(request.ParentName))
            student.FatherName = request.ParentName.Trim();

        if (!string.IsNullOrWhiteSpace(request.Phone))
            student.StudentPhone = request.Phone.Trim();

        var followup = student.ParentFollowup ?? await _db.ParentFollowups.FindAsync(id);
        if (followup == null)
        {
            followup = new ParentFollowup { StudentId = id };
            _db.ParentFollowups.Add(followup);
        }

        if (request.Address != null) followup.Address = request.Address;
        if (request.MaritalStatus != null) followup.MaritalStatus = request.MaritalStatus;

        if (request.HasHealthCondition.HasValue)
        {
            followup.HealthCondition = request.HasHealthCondition.Value ? "نعم" : "لا";
            followup.HealthDetails = request.HealthDetails ?? string.Empty;
        }
        else if (request.HealthDetails != null)
        {
            followup.HealthDetails = request.HealthDetails;
        }

        if (request.HasLearningDifficulties.HasValue)
        {
            followup.LearningDifficulties = request.HasLearningDifficulties.Value ? "نعم" : "لا";
            followup.LearningDifficultiesNotes = request.LearningDifficultiesDetails ?? string.Empty;
        }
        else if (request.LearningDifficultiesDetails != null)
        {
            followup.LearningDifficultiesNotes = request.LearningDifficultiesDetails;
        }

        await _db.SaveChangesAsync();

        student.ParentFollowup = followup;
        var today = DateTime.Today;
        var weekStart = AttendanceHelper.GetWeekStartSaturday(today);
        var weekEnd = weekStart.AddDays(6);
        var weekAttendances = await _db.CircleAttendances
            .Where(a => a.StudentId == id &&
                        a.AttendanceDateTime.Date >= weekStart &&
                        a.AttendanceDateTime.Date <= weekEnd)
            .ToListAsync();
        var todayDeparture = await _db.CircleDepartures
            .FirstOrDefaultAsync(d => d.StudentId == id && d.DepartureDate == DateOnly.FromDateTime(today));

        var profile = await _studentService.BuildProfileAsync(student, weekAttendances, todayDeparture, weekStart);
        return Ok(profile);
    }

    [HttpPost("{id:int}/photo")]
    [RequestSizeLimit(StudentPhotoStorage.MaxBytes)]
    public async Task<ActionResult<StudentProfileDto>> UploadPhoto(
        int id,
        IFormFile photo,
        CancellationToken cancellationToken)
    {
        var fatherPhone = GetFatherPhone();
        if (fatherPhone == null) return Unauthorized();

        var student = await _studentService.GetParentStudentByIdAsync(fatherPhone, id);
        if (student == null) return NotFound();

        if (photo.Length == 0)
            return BadRequest(new { message = "يرجى اختيار صورة" });

        var photoPath = await StudentPhotoStorage.SaveAsync(
            photo,
            _photoUploadOptions.Directory,
            cancellationToken);

        if (string.IsNullOrEmpty(photoPath))
            return BadRequest(new { message = "صيغة الصورة غير مدعومة أو حجمها كبير جداً (الحد الأقصى 1 م.ب)" });

        var followup = student.ParentFollowup ?? await _db.ParentFollowups.FindAsync([id], cancellationToken);
        if (followup == null)
        {
            followup = new ParentFollowup { StudentId = id };
            _db.ParentFollowups.Add(followup);
        }

        followup.PhotoPath = photoPath;
        await _db.SaveChangesAsync(cancellationToken);

        student.ParentFollowup = followup;
        var today = DateTime.Today;
        var weekStart = AttendanceHelper.GetWeekStartSaturday(today);
        var weekEnd = weekStart.AddDays(6);
        var weekAttendances = await _db.CircleAttendances
            .Where(a => a.StudentId == id &&
                        a.AttendanceDateTime.Date >= weekStart &&
                        a.AttendanceDateTime.Date <= weekEnd)
            .ToListAsync(cancellationToken);
        var todayDeparture = await _db.CircleDepartures
            .FirstOrDefaultAsync(
                d => d.StudentId == id && d.DepartureDate == DateOnly.FromDateTime(today),
                cancellationToken);

        var profile = await _studentService.BuildProfileAsync(
            student,
            weekAttendances,
            todayDeparture,
            weekStart);
        return Ok(profile);
    }

    private string? GetFatherPhone() => User.FindFirstValue("fatherPhone");
}
