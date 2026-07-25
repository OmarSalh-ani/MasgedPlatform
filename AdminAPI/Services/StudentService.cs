using AdminAPI.DTOs.Student;

using AdminAPI.Models;

using AdminAPI.Repositories.Interfaces;

using AdminAPI.Services.Interfaces;

using FluentValidation;

using Masged.WhatsApp;



namespace AdminAPI.Services;



public class StudentService(

    IStudentRepository repository,

    ICurrentUserContext currentUser,

    IValidator<SaveStudentRequestDto> saveValidator) : IStudentService

{

    public async Task<StudentFormDataDto> GetFormDataAsync(CancellationToken cancellationToken = default)

    {

        var circles = await repository.GetCirclesAsync(currentUser.IsGirlTeacher, cancellationToken);

        var planLevels = await repository.GetPlanLevelsAsync(cancellationToken);



        return new StudentFormDataDto

        {

            Circles = circles.Select(x => new StudentLookupOptionDto { Id = x.Id, Name = x.Name }).ToList(),

            PlanLevels = planLevels.Select(x => new StudentLookupOptionDto { Id = x.Id, Name = x.Name }).ToList(),

            CanModify = currentUser.CanModify,

            DefaultRegistrationDate = KuwaitTime.Now.ToString("yyyy-MM-dd"),

        };

    }



    public async Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)

    {

        var entity = await repository.GetByIdAsync(id, cancellationToken)

            ?? throw new KeyNotFoundException("لم يتم العثور على الطالب المحدد");



        EnsureCanAccessStudent(entity);

        return MapToDto(entity);

    }



    public async Task<StudentDto> CreateAsync(

        SaveStudentRequestDto request,

        CancellationToken cancellationToken = default)

    {

        EnsureCanModify();

        await saveValidator.ValidateAndThrowAsync(request, cancellationToken);



        var entity = new RegisterForm { CreatedAt = KuwaitTime.Now };

        ApplySave(entity, request, isCreate: true);



        await repository.AddAsync(entity, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);

    }



    public async Task<StudentDto> UpdateAsync(

        int id,

        SaveStudentRequestDto request,

        CancellationToken cancellationToken = default)

    {

        EnsureCanModify();

        await saveValidator.ValidateAndThrowAsync(request, cancellationToken);



        var entity = await repository.GetByIdAsync(id, cancellationToken)

            ?? throw new KeyNotFoundException("لم يتم العثور على الطالب المحدد");



        EnsureCanAccessStudent(entity);

        ApplySave(entity, request, isCreate: false);

        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);

    }



    private void EnsureCanAccessStudent(RegisterForm entity)

    {

        if (entity.StudentGender == "ذكر" && currentUser.IsGirlTeacher)

            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل هذا الطالب");

    }



    private void EnsureCanModify()

    {

        if (!currentUser.CanModify)

            throw new UnauthorizedAccessException("ليس لديك صلاحية لحفظ أو تعديل البيانات");

    }



    private static StudentDto MapToDto(RegisterForm entity) => new()

    {

        Id = entity.Id,

        StudentName = entity.StudentName,

        FullName = entity.FullName ?? entity.StudentName,

        FatherPhone = entity.FatherPhone,

        AlternativePhone = entity.FatherPhone2,

        ParentPanelPassword = entity.ThePassword,

        Age = entity.Age,

        StudentGender = entity.StudentGender,

        QuranCircleId = entity.QuranCircleId,

        PlanLevelId = entity.PlanLevelId,

        IsSpecial = entity.IsSpecial,

        CreatedAt = entity.CreatedAt,

    };



    private static void ApplySave(RegisterForm entity, SaveStudentRequestDto request, bool isCreate)

    {

        var fullName = request.FullName.Trim();

        entity.StudentName = fullName;

        entity.FatherName = fullName;

        entity.FullName = fullName;

        entity.FatherPhone = PhoneNormalizer.ToCanonical(request.FatherPhone);

        entity.FatherPhone2 = string.IsNullOrWhiteSpace(request.AlternativePhone)

            ? null

            : PhoneNormalizer.ToCanonical(request.AlternativePhone);

        entity.ThePassword = string.IsNullOrWhiteSpace(request.ParentPanelPassword)

            ? null

            : request.ParentPanelPassword.Trim();

        entity.StudentGender = request.StudentGender;

        entity.QuranCircleId = request.QuranCircleId;

        entity.PlanLevelId = request.PlanLevelId;

        entity.IsSpecial = request.IsSpecial;



        if (request.Age.HasValue)

            entity.Age = request.Age.Value;

        else if (isCreate)

            entity.Age = 0;

    }

}

