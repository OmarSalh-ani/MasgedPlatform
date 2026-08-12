using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Exceptions;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;

namespace AdminAPI.Services;

public class QuranCirclePlansClearService(
    IQuranCircleRepository repository,
    ICurrentUserContext currentUser) : IQuranCirclePlansClearService
{
    public async Task DeletePlansAsync(
        DeleteCirclePlansRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAdmin)
            throw new ForbiddenException("ليس لديك صلاحية لحذف خطط الحلقات");

        var circleIds = request.CircleIds.Distinct().ToList();
        await repository.DeletePlansAndArchiveForCirclesAsync(circleIds, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
