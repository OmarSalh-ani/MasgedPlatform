namespace Masged.WhatsApp.Options;

/// <summary>Optional runtime overrides (e.g. Admin Integrations DB) layered over appsettings/env.</summary>
public interface IWasenderRuntimeOverride
{
    string? ApiToken { get; }
    string? SessionApiKey { get; }
}
