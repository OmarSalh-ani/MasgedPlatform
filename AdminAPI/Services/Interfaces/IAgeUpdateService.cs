namespace AdminAPI.Services.Interfaces;

/// <summary>
/// Syncs <c>RegisterForm.Age</c> from <c>Birthdate</c> for all students that have a birthdate.
/// </summary>
public interface IAgeUpdateService
{
    Task UpdateAgesIfNeededAsync(CancellationToken cancellationToken = default);
}
