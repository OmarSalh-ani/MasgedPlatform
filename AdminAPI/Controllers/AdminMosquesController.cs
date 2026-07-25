using AdminAPI.DTOs.Common;

using AdminAPI.DTOs.Mosques;

using AdminAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;



namespace AdminAPI.Controllers;



[ApiController]

[Route("api/adminmosques")]

public class AdminMosquesController(IMosqueService mosqueService) : ControllerBase

{

    [HttpGet]

    public async Task<ActionResult<PagedResultDto<MosqueListItemDto>>> GetList(

        [FromQuery] int pageNumber = 1,

        [FromQuery] int pageSize = 0,

        CancellationToken cancellationToken = default)

    {

        var data = await mosqueService.GetListAsync(pageNumber, pageSize, cancellationToken);

        return Ok(data);

    }



    [HttpDelete("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(

        int id,

        CancellationToken cancellationToken = default)

    {

        var deleted = await mosqueService.DeleteAsync(id, cancellationToken);

        return Ok(new ApiResponseDto<bool>

        {

            Success = deleted,

            Message = deleted ? "تم الحذف" : "المسجد غير موجود",

            Data = deleted

        });

    }

}

