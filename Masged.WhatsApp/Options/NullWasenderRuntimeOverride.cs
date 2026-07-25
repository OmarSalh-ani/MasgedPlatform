using Masged.WhatsApp.Options;

namespace Masged.WhatsApp.Options;

public sealed class NullWasenderRuntimeOverride : IWasenderRuntimeOverride
{
    public string? ApiToken => null;
    public string? SessionApiKey => null;
}
