namespace AdminAPI.DTOs.Home;

public class TransferHomeStudentsRequestDto
{
    public List<int> StudentIds { get; set; } = [];
    public int CircleId { get; set; }
}
