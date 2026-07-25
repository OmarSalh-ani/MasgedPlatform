using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedTeacherMobileAPI.Entities;

[Table("TestBody")]
public class TestBody
{
    public int Id { get; set; }

    public int TestHeadId { get; set; }

    [StringLength(200)]
    public string? QuestionName { get; set; }

    public int QuestionOrder { get; set; }

    public int TestDegree { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TestHead? TestHead { get; set; }
}
