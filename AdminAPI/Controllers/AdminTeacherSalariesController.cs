using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSalaries;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminteachersalaries")]
public class AdminTeacherSalariesController(ITeacherSalaryService teacherSalaryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TeacherSalaryListItemDto>>> GetList(
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] int? teacherId,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.GetListAsync(month, year, teacherId, cancellationToken);
        return Ok(data);
    }

    [HttpGet("filter-options")]
    public async Task<ActionResult<ApiResponseDto<TeacherSalaryFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.GetFilterOptionsAsync(cancellationToken);
        return Ok(new ApiResponseDto<TeacherSalaryFilterOptionsDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("form-teachers")]
    public async Task<ActionResult<ApiResponseDto<List<TeacherSalaryFormTeacherDto>>>> GetFormTeachers(
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.GetFormTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<TeacherSalaryFormTeacherDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("report")]
    public async Task<ActionResult<ApiResponseDto<TeacherSalaryReportDto>>> GetReport(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.GetReportAsync(month, year, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSalaryReportDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("report/export")]
    public async Task<IActionResult> ExportReport(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var bytes = await teacherSalaryService.ExportReportExcelAsync(month, year, cancellationToken);
        var fileName = $"TeacherSalaryReport_{year}_{month}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherSalaryDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSalaryDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TeacherSalaryDto>>> Create(
        [FromBody] SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSalaryDto>
        {
            Success = true,
            Message = "تم حفظ الراتب بنجاح",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherSalaryDto>>> Update(
        int id,
        [FromBody] SaveTeacherSalaryRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSalaryDto>
        {
            Success = true,
            Message = "تم حفظ الراتب بنجاح",
            Data = data,
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await teacherSalaryService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الراتب غير موجود",
            Data = deleted,
        });
    }

    [HttpPost("calculate-attendance")]
    public async Task<ActionResult<ApiResponseDto<AttendanceCalculationResultDto>>> CalculateAttendance(
        [FromBody] CalculateTeacherAttendanceRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.CalculateAttendanceAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<AttendanceCalculationResultDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("calculate-salary")]
    public async Task<ActionResult<ApiResponseDto<SalaryCalculationResultDto>>> CalculateSalary(
        [FromBody] CalculateTeacherSalaryRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.CalculateSalaryAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<SalaryCalculationResultDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("auto-calculate")]
    public async Task<ActionResult<ApiResponseDto<AutoCalculateMonthResultDto>>> AutoCalculate(
        [FromBody] AutoCalculateMonthRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.AutoCalculateAllForMonthAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<AutoCalculateMonthResultDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("pay")]
    public async Task<ActionResult<ApiResponseDto<PaySelectedSalariesResultDto>>> PaySelected(
        [FromBody] PaySelectedSalariesRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await teacherSalaryService.PaySelectedSalariesAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<PaySelectedSalariesResultDto>
        {
            Success = true,
            Message = data.Message,
            Data = data,
        });
    }
}
