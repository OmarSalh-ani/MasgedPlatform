using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSendNotes;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/adminteachersendnotes")]
public class AdminTeacherSendNotesController(ITeacherSendNoteService teacherSendNoteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TeacherSendNoteListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = TeacherSendNoteService.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var data = await teacherSendNoteService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("teachers")]
    public async Task<ActionResult<ApiResponseDto<List<TeacherOptionDto>>>> GetTeachers(
        CancellationToken cancellationToken = default)
    {
        var data = await teacherSendNoteService.GetTeachersAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<TeacherOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherSendNoteDto>>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await teacherSendNoteService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSendNoteDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<int>>> Create(
        [FromBody] CreateTeacherSendNotesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var count = await teacherSendNoteService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<int>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = count
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherSendNoteDto>>> Update(
        int id,
        [FromBody] UpdateTeacherSendNoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await teacherSendNoteService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<TeacherSendNoteDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await teacherSendNoteService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "الملاحظة غير موجودة",
            Data = deleted
        });
    }
}
