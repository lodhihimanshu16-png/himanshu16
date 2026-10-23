namespace LearnHub.Api.Models;

public class CartItem
{
    public string StudentName { get; set; } = "";
    public int CourseId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
