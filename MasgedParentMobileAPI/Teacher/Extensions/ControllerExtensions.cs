using MasgedTeacherMobileAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace MasgedTeacherMobileAPI.Extensions;

public static class ControllerExtensions
{
    public static IActionResult ToActionResult(this ControllerBase controller, GlobalResponse response) =>
        controller.StatusCode((int)response.StatusCode, response);
}
