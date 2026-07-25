using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminstudentplan")]
public class AdminStudentPlanController(IStudentPlanService studentPlanService) : ControllerBase
{
    [HttpGet("form-data")]
    public async Task<ActionResult<ApiResponseDto<StudentPlanFormDataDto>>> GetFormData(
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.GetFormDataAsync(cancellationToken);
        return Ok(ApiOk(data));
    }

    [HttpGet("ayahs/{surahId:int}")]
    public async Task<ActionResult<ApiResponseDto<List<StudentPlanAyahDto>>>> GetAyahs(
        int surahId,
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.GetAyahsAsync(surahId, cancellationToken);
        return Ok(ApiOk(data));
    }

    [HttpGet("students/{studentId:int}/resolve")]
    public async Task<ActionResult<ApiResponseDto<StudentPlanResolveDto>>> Resolve(
        int studentId,
        [FromQuery] int? planId,
        [FromQuery] string? edit,
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.ResolveAsync(studentId, planId, edit, cancellationToken);
        return Ok(ApiOk(data));
    }

    [HttpGet("students/{studentId:int}/plans/{planId:int}")]
    public async Task<ActionResult<ApiResponseDto<StudentPlanDetailDto>>> GetPlanDetail(
        int studentId,
        int planId,
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.GetPlanDetailAsync(studentId, planId, cancellationToken);
        return Ok(ApiOk(data));
    }

    [HttpGet("edit-prefill")]
    public async Task<ActionResult<ApiResponseDto<StudentPlanEditPrefillDto>>> GetEditPrefill(
        [FromQuery] string editKey,
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.GetEditPrefillAsync(editKey, cancellationToken);
        if (data is null)
            return NotFound(ApiFail<StudentPlanEditPrefillDto>("البند غير موجود"));
        return Ok(ApiOk(data));
    }

    [HttpPost("plans")]
    public async Task<ActionResult<ApiResponseDto<CreateStudentPlanResponseDto>>> CreatePlan(
        [FromBody] CreateStudentPlanRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await studentPlanService.CreatePlanAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<CreateStudentPlanResponseDto>
        {
            Success = true,
            Message = string.Empty,
            Data = data,
        });
    }

    [HttpPost("save")]
    public async Task<ActionResult<ApiResponseDto<bool>>> SavePlan(
        [FromBody] SaveStudentPlanRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await studentPlanService.SavePlanAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "تم حفظ الخطة بنجاح.",
            Data = true,
        });
    }

    [HttpPut("items")]
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateItem(
        [FromBody] UpdateStudentPlanItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await studentPlanService.UpdateSingleItemAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "تم تحديث الخطة بنجاح.",
            Data = true,
        });
    }

    [HttpDelete("items/{editKey}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteItem(
        string editKey,
        CancellationToken cancellationToken = default)
    {
        var deleted = await studentPlanService.DeleteItemAsync(editKey, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? string.Empty : "البند غير موجود",
            Data = deleted,
        });
    }

    private static ApiResponseDto<T> ApiOk<T>(T data) =>
        new() { Success = true, Data = data };

    private static ApiResponseDto<T> ApiFail<T>(string message) =>
        new() { Success = false, Message = message };
}
