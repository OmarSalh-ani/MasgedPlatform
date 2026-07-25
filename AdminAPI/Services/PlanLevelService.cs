using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.PlanLevels;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace AdminAPI.Services;

public class PlanLevelService(
    IPlanLevelRepository repository,
    IMapper mapper) : IPlanLevelService
{
    public async Task<PagedResultDto<PlanLevelListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var (items, totalCount) = await repository.GetPagedAsync(page, pageSize, cancellationToken);
        var effectivePageSize = pageSize <= 0 ? (totalCount == 0 ? 1 : totalCount) : pageSize;

        return new PagedResultDto<PlanLevelListItemDto>
        {
            Items = mapper.Map<List<PlanLevelListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = effectivePageSize,
            TotalPages = pageSize <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<PlanLevelDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المستوى غير موجود");

        return mapper.Map<PlanLevelDto>(entity);
    }

    public async Task<PlanLevelDto> CreateAsync(
        SavePlanLevelRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = new PlanLevel
        {
            LevelName = request.LevelName.Trim(),
            UnitType = request.UnitType,
            Quantity = request.Quantity,
            CreatedAt = KuwaitTime.Now
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return mapper.Map<PlanLevelDto>(entity);
    }

    public async Task<PlanLevelDto> UpdateAsync(
        int id,
        SavePlanLevelRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المستوى غير موجود");

        entity.LevelName = request.LevelName.Trim();
        entity.UnitType = request.UnitType;
        entity.Quantity = request.Quantity;

        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<PlanLevelDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (await repository.HasReadyPlanDependencyAsync(id, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    string.Empty,
                    "لا يمكن حذف هذا المستوى لوجود خطط جاهزة مرتبطة به. يرجى حذف الخطط الجاهزة أولاً.")
            ]);
        }

        if (await repository.HasRegisterFormDependencyAsync(id, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    string.Empty,
                    "لا يمكن حذف هذا المستوى لوجود طلاب مسجلين عليه. يرجى تغيير مستوى الطلاب أولاً.")
            ]);
        }

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ValidationException([
                new ValidationFailure(string.Empty, $"حدث خطأ أثناء الحذف: {ex.Message}")
            ]);
        }

        return true;
    }
}
