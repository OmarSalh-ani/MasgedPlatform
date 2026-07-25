using AdminAPI.DTOs.Common;

using AdminAPI.DTOs.HeroSlides;

using AdminAPI.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;



namespace AdminAPI.Controllers;



[ApiController]

[Route("api/adminheroslides")]

public class AdminHeroSlidesController(IHeroSlideService heroSlideService) : ControllerBase

{

    [HttpGet]

    public async Task<ActionResult<PagedResultDto<HeroSlideListItemDto>>> GetList(

        [FromQuery] int pageNumber = 1,

        [FromQuery] int pageSize = 0,

        CancellationToken cancellationToken = default)

    {

        var data = await heroSlideService.GetListAsync(pageNumber, pageSize, cancellationToken);

        return Ok(data);

    }



    [HttpGet("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<HeroSlideDto>>> GetById(

        int id,

        CancellationToken cancellationToken = default)

    {

        var data = await heroSlideService.GetByIdAsync(id, cancellationToken);

        return Ok(new ApiResponseDto<HeroSlideDto>

        {

            Success = true,

            Message = "OK",

            Data = data

        });

    }



    [HttpGet("next-sort-order")]

    public async Task<ActionResult<ApiResponseDto<int>>> GetNextSortOrder(

        CancellationToken cancellationToken = default)

    {

        var data = await heroSlideService.GetNextSortOrderAsync(cancellationToken);

        return Ok(new ApiResponseDto<int>

        {

            Success = true,

            Message = "OK",

            Data = data

        });

    }



    [HttpPost]

    public async Task<ActionResult<ApiResponseDto<HeroSlideDto>>> Create(

        [FromForm] SaveHeroSlideRequestDto request,

        CancellationToken cancellationToken = default)

    {

        var data = await heroSlideService.CreateAsync(request, cancellationToken);

        return Ok(new ApiResponseDto<HeroSlideDto>

        {

            Success = true,

            Message = "تم الحفظ",

            Data = data

        });

    }



    [HttpPut("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<HeroSlideDto>>> Update(

        int id,

        [FromForm] SaveHeroSlideRequestDto request,

        CancellationToken cancellationToken = default)

    {

        var data = await heroSlideService.UpdateAsync(id, request, cancellationToken);

        return Ok(new ApiResponseDto<HeroSlideDto>

        {

            Success = true,

            Message = "تم الحفظ",

            Data = data

        });

    }



    [HttpDelete("{id:int}")]

    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(

        int id,

        [FromQuery] bool deleteImageFile = false,

        CancellationToken cancellationToken = default)

    {

        var deleted = await heroSlideService.DeleteAsync(id, deleteImageFile, cancellationToken);

        return Ok(new ApiResponseDto<bool>

        {

            Success = deleted,

            Message = deleted ? "تم الحذف" : "الصورة غير موجودة",

            Data = deleted

        });

    }

}

