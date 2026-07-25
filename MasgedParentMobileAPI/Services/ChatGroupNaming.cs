namespace MasgedParentMobileAPI.Services;

public static class ChatGroupNaming
{
    public static string For(int studentId, int teacherId) =>
        $"chat_{studentId}_{teacherId}";
}
