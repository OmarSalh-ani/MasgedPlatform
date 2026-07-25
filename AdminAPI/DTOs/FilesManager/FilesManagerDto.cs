namespace AdminAPI.DTOs.FilesManager;

public class FilesManagerDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;
}
