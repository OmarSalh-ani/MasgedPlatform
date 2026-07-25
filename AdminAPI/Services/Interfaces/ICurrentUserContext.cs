namespace AdminAPI.Services.Interfaces;

public interface ICurrentUserContext
{
    int TeacherId { get; }
    bool IsGirlTeacher { get; }
    bool IsAdmin { get; }
    bool CanModify { get; }
}
