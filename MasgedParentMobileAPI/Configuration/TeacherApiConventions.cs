using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MasgedParentMobileAPI.Configuration;

/// <summary>
/// Applies <c>api/teacher/[controller]</c> routes and TeacherJwt auth to controllers
/// from the <c>MasgedTeacherMobileAPI</c> namespace (under <c>Teacher/</c>).
/// </summary>
public sealed class TeacherApiApplicationModelProvider : IApplicationModelProvider
{
    private const string TeacherNamespacePrefix = "MasgedTeacherMobileAPI";

    // Runs after AuthorizationApplicationModelProvider (-980) so default [Authorize] is stripped.
    public int Order => -979;

    public void OnProvidersExecuted(ApplicationModelProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        foreach (var controller in context.Result.Controllers)
        {
            var ns = controller.ControllerType.Namespace;
            if (ns == null || !ns.StartsWith(TeacherNamespacePrefix, StringComparison.Ordinal))
                continue;

            RemoveDefaultAuthorizeFilters(controller.Filters);
            foreach (var action in controller.Actions)
            {
                RemoveDefaultAuthorizeFilters(action.Filters);
            }

            controller.Filters.Add(new AuthorizeFilter("TeacherOnly"));

            foreach (var selector in controller.Selectors)
            {
                var template = selector.AttributeRouteModel?.Template;
                if (template == null) continue;

                if (template.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(
                        new RouteAttribute($"api/teacher/{template["api/".Length..]}"));
                }
                else if (!template.StartsWith("api/teacher/", StringComparison.OrdinalIgnoreCase))
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(
                        new RouteAttribute($"api/teacher/{template}"));
                }
            }
        }
    }

    private static void RemoveDefaultAuthorizeFilters(IList<IFilterMetadata> filters)
    {
        for (var i = filters.Count - 1; i >= 0; i--)
        {
            if (filters[i] is AuthorizeFilter)
                filters.RemoveAt(i);
        }
    }
}
