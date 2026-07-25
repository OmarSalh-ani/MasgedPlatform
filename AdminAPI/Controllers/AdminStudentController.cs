using AdminAPI.DTOs.Common;

using AdminAPI.DTOs.Student;

using AdminAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;



namespace AdminAPI.Controllers;



[ApiController]

[Authorize]

[Route("api/adminstudent")]

public class AdminStudentController(IStudentService studentService) : ControllerBase

{

    [HttpGet("form-data")]

    public async Task<ActionResult<ApiResponseDto<StudentFormDataDto>>> GetFormData(

        CancellationToken cancellationToken)

    {

        var data = await studentService.GetFormDataAsync(cancellationToken);

        return Ok(new ApiResponseDto<StudentFormDataDto>

        {

            Success = true,

            Message = "OK",

            Data = data,

        });

    }



    [HttpGet("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetById(

        int id,

        CancellationToken cancellationToken)

    {

        var data = await studentService.GetByIdAsync(id, cancellationToken);

        return Ok(new ApiResponseDto<StudentDto>

        {

            Success = true,

            Message = "OK",

            Data = data,

        });

    }



    [HttpPost]

    public async Task<ActionResult<ApiResponseDto<StudentDto>>> Create(

        [FromBody] SaveStudentRequestDto request,

        CancellationToken cancellationToken)

    {

        var data = await studentService.CreateAsync(request, cancellationToken);

        return Ok(new ApiResponseDto<StudentDto>

        {

            Success = true,

            Message = "تم حفظ بيانات الطالب بنجاح",

            Data = data,

        });

    }



    [HttpPut("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<StudentDto>>> Update(

        int id,

        [FromBody] SaveStudentRequestDto request,

        CancellationToken cancellationToken)

    {

        var data = await studentService.UpdateAsync(id, request, cancellationToken);

        return Ok(new ApiResponseDto<StudentDto>

        {

            Success = true,

            Message = "تم حفظ بيانات الطالب بنجاح",

            Data = data,

        });

    }

}

