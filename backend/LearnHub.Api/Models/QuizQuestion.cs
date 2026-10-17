namespace LearnHub.Api.Models;

public class QuizQuestion
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = new();
    public int CorrectIndex { get; set; }
}
