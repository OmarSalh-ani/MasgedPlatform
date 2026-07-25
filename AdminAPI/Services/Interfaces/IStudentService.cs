using AdminAPI.DTOs.Student;



namespace AdminAPI.Services.Interfaces;



public interface IStudentService

{

    Task<StudentFormDataDto> GetFormDataAsync(CancellationToken cancellationToken = default);



    Task<StudentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);



    Task<StudentDto> CreateAsync(SaveStudentRequestDto request, CancellationToken cancellationToken = default);



    Task<StudentDto> UpdateAsync(

        int id,

        SaveStudentRequestDto request,

        CancellationToken cancellationToken = default);

}

