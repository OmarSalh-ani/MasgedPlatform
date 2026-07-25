using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.WhatsappSender;

namespace AdminAPI.Services.Interfaces;

public interface IWhatsappSenderService
{
    Task<PagedResultDto<HomeStudentListItemDto>> GetListAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<HomeFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default);

    Task<List<WhatsappSenderFormOptionDto>> GetFormOptionsAsync(CancellationToken cancellationToken = default);

    Task<string> SendWhatsappAsync(
        SendWhatsappSenderRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default);
}
