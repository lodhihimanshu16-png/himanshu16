namespace LearnHub.Api.Models;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int DurationMinutes { get; set; }
    public string Content { get; set; } = "";
}
