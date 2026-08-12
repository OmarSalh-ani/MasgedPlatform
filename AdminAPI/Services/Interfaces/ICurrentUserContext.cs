namespace AdminAPI.Services.Interfaces;

public interface ICurrentUserContext
{
    int TeacherId { get; }
    bool IsGirlTeacher { get; }
    bool IsAdmin { get; }
    bool IsSupervisor { get; }
    bool CanModify { get; }
}
