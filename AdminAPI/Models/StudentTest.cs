namespace AdminAPI.Models;

public class StudentTest
{
    public int Id { get; set; }
    public int StudentId { get; set; }

    public virtual RegisterForm? Student { get; set; }
}
