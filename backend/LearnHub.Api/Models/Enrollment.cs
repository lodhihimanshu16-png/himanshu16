namespace LearnHub.Api.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string StudentName { get; set; } = "";
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
