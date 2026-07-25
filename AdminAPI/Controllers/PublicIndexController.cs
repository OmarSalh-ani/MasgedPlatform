using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.CountryDialCodes;
using AdminAPI.DTOs.PublicIndex;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/publicindex")]
public class PublicIndexController(IPublicIndexService publicIndexService) : ControllerBase
{
    [HttpGet("content")]
    public async Task<ActionResult<ApiResponseDto<PublicWebsiteContentDto>>> GetContent(
        CancellationToken cancellationToken = default)
    {
        var data = await publicIndexService.GetWebsiteContentAsync(cancellationToken);
        return Ok(new ApiResponseDto<PublicWebsiteContentDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("registration-config")]
    public async Task<ActionResult<ApiResponseDto<PublicRegistrationConfigDto>>> GetRegistrationConfig(
        [FromQuery] string? mode,
        CancellationToken cancellationToken = default)
    {
        var data = await publicIndexService.GetRegistrationConfigAsync(mode, cancellationToken);
        return Ok(new ApiResponseDto<PublicRegistrationConfigDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpPost("registration")]
    public async Task<ActionResult<ApiResponseDto<SubmitPublicRegistrationResponseDto>>> SubmitRegistration(
        [FromBody] SubmitPublicRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var data = await publicIndexService.SubmitRegistrationAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<SubmitPublicRegistrationResponseDto>
        {
            Success = true,
            Message = "تم التسجيل بنجاح",
            Data = data,
        });
    }

    [HttpGet("register-success")]
    public async Task<ActionResult<ApiResponseDto<PublicRegisterSuccessDto>>> GetRegisterSuccess(
        CancellationToken cancellationToken = default)
    {
        var data = await publicIndexService.GetRegisterSuccessAsync(cancellationToken);
        return Ok(new ApiResponseDto<PublicRegisterSuccessDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}

[ApiController]
[AllowAnonymous]
[Route("api/publiccountrydialcodes")]
public class PublicCountryDialCodesController(ICountryDialCodeService countryDialCodeService) : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponseDto<IReadOnlyList<CountryDialEntryDto>>> GetList()
    {
        var data = countryDialCodeService.GetCountries();
        return Ok(new ApiResponseDto<IReadOnlyList<CountryDialEntryDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
