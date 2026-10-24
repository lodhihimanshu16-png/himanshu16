using System.ComponentModel;
using ModelContextProtocol.Server;

namespace LearnHub.Mcp.Tools;

[McpServerToolType]
public sealed class CatalogTools(Db db)
{
    [McpServerTool(Name = "get_schema", ReadOnly = true), Description("List every LearnHub table with its columns and types.")]
    public Task<List<Dictionary<string, object?>>> GetSchema() => db.QueryAsync(
        @"SELECT table_name AS `table`, column_name AS `column`, column_type AS type, is_nullable AS nullable
          FROM information_schema.columns WHERE table_schema = 'learnhub'
          ORDER BY table_name, ordinal_position", Db.MaxRows);

    [McpServerTool(Name = "list_courses", ReadOnly = true), Description("List courses with instructor, price and average rating. Optional category/level filters.")]
    public Task<List<Dictionary<string, object?>>> ListCourses(
        [Description("Exact category name")] string? category = null,
        [Description("Level, e.g. Beginner")] string? level = null,
        [Description("Max rows (default 100)")] int limit = 100) => db.QueryAsync(
        @"SELECT c.id, c.title, c.category, c.level, c.price, i.full_name AS instructor,
                 ROUND(AVG(r.stars), 2) AS avg_stars, COUNT(r.id) AS review_count
          FROM courses c
          JOIN instructors i ON i.id = c.instructor_id
          LEFT JOIN reviews r ON r.course_id = c.id
          WHERE (@category IS NULL OR c.category = @category) AND (@level IS NULL OR c.level = @level)
          GROUP BY c.id, i.full_name ORDER BY c.id", limit, ("@category", category), ("@level", level));

    [McpServerTool(Name = "get_course", ReadOnly = true), Description("Get one course with lesson count, enrolments and prerequisites.")]
    public Task<List<Dictionary<string, object?>>> GetCourse([Description("Course id")] int id) => db.QueryAsync(
        @"SELECT c.id, c.title, c.short_description, c.category, c.level, c.price, c.instructor_id,
                 (SELECT COUNT(*) FROM lessons l WHERE l.course_id = c.id) AS lessons,
                 (SELECT COUNT(*) FROM enrollments e WHERE e.course_id = c.id) AS enrollments,
                 (SELECT GROUP_CONCAT(p.requires_course_id) FROM course_prerequisites p WHERE p.course_id = c.id) AS prerequisite_course_ids
          FROM courses c WHERE c.id = @id", 1, ("@id", id));
}
