namespace MasgedParentMobileAPI.DTOs;

public class StudentRegistrationResponseDto : LoginResponse
{
    public List<int> StudentIds { get; set; } = [];
}
