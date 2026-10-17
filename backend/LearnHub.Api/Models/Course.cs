namespace LearnHub.Api.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string Category { get; set; } = "";
    public string Instructor { get; set; } = "";
    public string Level { get; set; } = "";
    public List<Lesson> Lessons { get; set; } = new();
}
