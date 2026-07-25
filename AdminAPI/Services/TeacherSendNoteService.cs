using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSendNotes;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;

namespace AdminAPI.Services;

public class TeacherSendNoteService(
    ITeacherSendNoteRepository repository,
    IMapper mapper) : ITeacherSendNoteService
{
    public const int DefaultPageSize = 10;

    public async Task<PagedResultDto<TeacherSendNoteListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? DefaultPageSize : pageSize;
        var (items, totalCount) = await repository.GetPagedAsync(page, size, cancellationToken);

        return new PagedResultDto<TeacherSendNoteListItemDto>
        {
            Items = mapper.Map<List<TeacherSendNoteListItemDto>>(items),
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size,
            TotalPages = (int)Math.Ceiling(totalCount / (double)size)
        };
    }

    public async Task<List<TeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default)
    {
        var teachers = await repository.GetTeachersOrderedByNameAsync(cancellationToken);
        return mapper.Map<List<TeacherOptionDto>>(teachers);
    }

    public async Task<TeacherSendNoteDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الملاحظة غير موجودة");

        return mapper.Map<TeacherSendNoteDto>(entity);
    }

    public async Task<int> CreateAsync(
        CreateTeacherSendNotesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.Now;
        var noteText = request.Note.Trim();
        var notes = request.TeacherIds
            .Distinct()
            .Select(teacherId => new TeachersAdminNote
            {
                TeacherId = teacherId,
                Note = noteText,
                CreatedAt = now,
                IsRead = false
            })
            .ToList();

        await repository.AddRangeAsync(notes, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return notes.Count;
    }

    public async Task<TeacherSendNoteDto> UpdateAsync(
        int id,
        UpdateTeacherSendNoteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("الملاحظة غير موجودة");

        entity.Note = request.Note.Trim();
        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<TeacherSendNoteDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return false;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
