using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Options;
using MasgedTeacherMobileAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Configuration;

public static class UnifiedApiExtensions
{
    /// <summary>
    /// Registers teacher mobile API DbContext and services for the unified host.
    /// </summary>
    public static IServiceCollection AddUnifiedTeacherMobileApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ChatSettings>(configuration.GetSection("Chat"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient<ChatBroadcastClient>();
        services.AddScoped<MasgedTeacherMobileAPI.Services.ChatService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("TeacherOnly", policy =>
            {
                policy.AddAuthenticationSchemes("TeacherJwt");
                policy.RequireAuthenticatedUser();
            });
        });

        return services;
    }
}
