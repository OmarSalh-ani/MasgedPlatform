namespace AdminAPI.DTOs.PublicIndex;

public class PublicWomanActivityOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PublicRegistrationFormLabelsDto
{
    public string FullNameLabel { get; set; } = "الاسم الرباعي للطالب *";
    public string ParentPhone1Label { get; set; } = "رقم هاتف ولي الأمر 1 *";
    public string LearnCertificateLabel { get; set; } = "المؤهل العلمي";
    public bool ShowLearnDiv { get; set; }
    public bool ShowBirthdateDiv { get; set; } = true;
    public bool ShowAgeDiv { get; set; }
    public bool ShowPhone2Div { get; set; } = true;
    public bool ShowActivitiesSection { get; set; } = true;
    public bool ShowActivitiesNav { get; set; } = true;
}

public class PublicRegistrationConfigDto
{
    public string Mode { get; set; } = "default";
    public bool RegistrationEnabled { get; set; }
    public PublicRegistrationFormLabelsDto Labels { get; set; } = new();
    public List<PublicWomanActivityOptionDto> WomanActivities { get; set; } = [];
}

public class SubmitPublicRegistrationRequestDto
{
    public string Mode { get; set; } = "default";
    public string FullName { get; set; } = string.Empty;
    public DateTime? Birthdate { get; set; }
    public int? Age { get; set; }
    public string ParentPhoneCountryIso { get; set; } = string.Empty;
    public string ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; }
    public string? ParentPhone2CountryIso { get; set; }
    public string? LearnCertificate { get; set; }
    public int WomanActivityTypeId { get; set; }
}

public class SubmitPublicRegistrationResponseDto
{
    public int Id { get; set; }
}

public class PublicRegisterSuccessDto
{
    public string HeadText { get; set; } = "تم تسجيل البيانات بنجاح لحلقة تحفيظ القرآن الكريم";
    public string TitleText { get; set; } = string.Empty;
    public string SubscribeText { get; set; } = string.Empty;
    public string WhatsappUrl { get; set; } = string.Empty;
    public List<PublicSocialLinkItemDto> SocialLinks { get; set; } = [];
}
