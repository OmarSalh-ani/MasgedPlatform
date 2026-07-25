using AdminAPI.DTOs.PublicIndex;

namespace AdminAPI.Services;

public partial class PublicIndexService
{
    private async Task<PublicRegistrationConfigDto> BuildRegistrationConfigAsync(
        string mode,
        CancellationToken cancellationToken)
    {
        var forGirl = mode == "wregister";
        var registrationEnabled = await registrationSettings.GetRegistrationEnabledAsync(forGirl, cancellationToken);
        var labels = BuildFormLabels(mode);

        return new PublicRegistrationConfigDto
        {
            Mode = mode,
            RegistrationEnabled = registrationEnabled,
            Labels = labels,
            WomanActivities = await GetWomanActivitiesAsync(forGirl, cancellationToken),
        };
    }

    private static PublicRegistrationFormLabelsDto BuildFormLabels(string mode)
    {
        var labels = new PublicRegistrationFormLabelsDto();

        if (mode is "mregister" or "wregister")
        {
            labels.ShowActivitiesSection = false;
            labels.ShowActivitiesNav = false;
        }

        if (mode == "default")
        {
            labels.ShowLearnDiv = false;
            return labels;
        }

        if (mode == "mregister")
        {
            labels.ShowLearnDiv = false;
            return labels;
        }

        labels.FullNameLabel = "الاسم الثلاثي *";
        labels.ParentPhone1Label = "رقم الهاتف *";
        labels.LearnCertificateLabel = "المرحلة الدراسية";
        labels.ShowAgeDiv = true;
        labels.ShowBirthdateDiv = false;
        labels.ShowPhone2Div = false;
        return labels;
    }
}
