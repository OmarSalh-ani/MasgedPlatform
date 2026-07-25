using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.CurrentStudentsPlans;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admincurrentstudentsplans")]
public class AdminCurrentStudentsPlansController(ICurrentStudentPlanService currentStudentPlanService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<CurrentStudentPlanListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? studentId = null,
        CancellationToken cancellationToken = default)
    {
        var data = await currentStudentPlanService.GetListAsync(
            pageNumber,
            pageSize,
            studentId,
            cancellationToken);
        return Ok(data);
    }

    [HttpGet("students")]
    public async Task<ActionResult<PagedResultDto<CurrentStudentPlanStudentLookupDto>>> GetStudents(
        [FromQuery] CurrentStudentPlanStudentLookupFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await currentStudentPlanService.GetStudentsAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await currentStudentPlanService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم حذف الخطة بنجاح." : "الخطة غير موجودة",
            Data = deleted
        });
    }
}
