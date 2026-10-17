namespace LearnHub.Api.Models;

public class LessonProgress
{
    public string StudentName { get; set; } = "";
    public int CourseId { get; set; }
    public int LessonId { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
