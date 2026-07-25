using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.TeacherSendNotes;

namespace AdminAPI.Services.Interfaces;

public interface ITeacherSendNoteService
{
    Task<PagedResultDto<TeacherSendNoteListItemDto>> GetListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<List<TeacherOptionDto>> GetTeachersAsync(CancellationToken cancellationToken = default);

    Task<TeacherSendNoteDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateTeacherSendNotesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<TeacherSendNoteDto> UpdateAsync(
        int id,
        UpdateTeacherSendNoteRequestDto request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
