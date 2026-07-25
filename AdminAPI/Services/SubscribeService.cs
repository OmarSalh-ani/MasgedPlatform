using AdminAPI.DTOs.Subscribe;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;

namespace AdminAPI.Services;

public class SubscribeService(ISubscribeRepository repository) : ISubscribeService
{
    public async Task<SubmitSubscribeResponseDto> SubmitAsync(
        SubmitSubscribeRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var fullName = request.FullName.Trim();
        var mobile = request.Mobile.Trim();

        if (await repository.MobileExistsAsync(mobile, cancellationToken))
            throw new ValidationException("هذا الرقم مسجل مسبقاً");

        var contact = await repository.AddContactAsync(
            new AnnouncementContact
            {
                Name = fullName,
                Mobile = mobile,
                CreatedAt = KuwaitTime.Now,
            },
            cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        var welcomeMessage = BuildWelcomeMessage(fullName);
        await repository.AddMessageAsync(
            new AnnouncementMessage
            {
                ContactId = contact.Id > 0 ? contact.Id : null,
                Mobile = mobile,
                Message = welcomeMessage,
                Image = null,
                SentAt = KuwaitTime.Now,
                Status = "Pending",
                IsProcessed = false,
            },
            cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return new SubmitSubscribeResponseDto { Registered = true };
    }

    private static string BuildWelcomeMessage(string fullName) =>
        $@"السلام عليكم ورحمة الله وبركاته 

*حياك الله*
{fullName}
نرحّب بك في خدمة رسائل مسجد الشيخ مبارك عبدالله المبارك الصباح للدروس والمحاضرات والدورات العلمية.

سوف يتم إرسال لك كل مواعيد النشاطات عبر الواتساب. 

1. حفظ الرقم.
2. إرسال كلمة (`تم`).";
}
