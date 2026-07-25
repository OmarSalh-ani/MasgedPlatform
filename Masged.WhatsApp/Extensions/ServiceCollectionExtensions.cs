using Microsoft.Extensions.DependencyInjection.Extensions;
using Masged.WhatsApp.Interfaces;
using Masged.WhatsApp.Options;
using Masged.WhatsApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Masged.WhatsApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasgedWhatsApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<WasenderApiOptions>(
            configuration.GetSection(WasenderApiOptions.SectionName));
        services.Configure<WhatsAppProcessorOptions>(
            configuration.GetSection(WhatsAppProcessorOptions.SectionName));
        services.TryAddSingleton<IWasenderRuntimeOverride, NullWasenderRuntimeOverride>();

        services.AddHttpClient<IWasenderApiClient, WasenderApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<WasenderApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Masged.WhatsApp/1.0");
        });

        services.AddHttpClient<IWasenderSessionClient, WasenderSessionClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<WasenderApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Masged.WhatsApp/1.0");
        });

        services.AddScoped<WasenderSessionKeySyncService>();
        services.AddHostedService<WhatsAppBackgroundService>();

        return services;
    }
}
