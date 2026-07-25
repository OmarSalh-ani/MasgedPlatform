namespace AdminAPI.DTOs.FilesManager;

public class SaveFilesManagerRequestDto
{
    public string Name { get; set; } = string.Empty;

    public IFormFile? File { get; set; }
}
