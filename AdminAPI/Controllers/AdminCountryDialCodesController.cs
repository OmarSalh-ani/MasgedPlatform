using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.CountryDialCodes;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admincountrydialcodes")]
public class AdminCountryDialCodesController(ICountryDialCodeService countryDialCodeService) : ControllerBase
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
