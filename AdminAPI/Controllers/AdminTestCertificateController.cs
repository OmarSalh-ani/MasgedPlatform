using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TestCertificate;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/admintestcertificate")]
public class AdminTestCertificateController(ITestCertificateService testCertificateService) : ControllerBase
{
    [HttpGet("{testId:int}")]
    public async Task<ActionResult<ApiResponseDto<TestCertificateDto>>> GetByTestId(
        int testId,
        CancellationToken cancellationToken)
    {
        var data = await testCertificateService.GetByTestIdAsync(testId, cancellationToken);
        return Ok(new ApiResponseDto<TestCertificateDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }
}
