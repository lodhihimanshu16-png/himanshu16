using System.ComponentModel;
using ModelContextProtocol.Server;

namespace LearnHub.Mcp.Tools;

[McpServerToolType]
public sealed class StudentTools(Db db)
{
    [McpServerTool(Name = "get_student", ReadOnly = true), Description("Look up students by id or by name/email fragment.")]
    public Task<List<Dictionary<string, object?>>> GetStudent(
        [Description("Student id")] int? id = null,
        [Description("Name or email fragment")] string? search = null,
        int limit = 25) => db.QueryAsync(
        @"SELECT s.id, s.full_name, s.email, s.joined_at,
                 (SELECT COUNT(*) FROM enrollments e WHERE e.student_id = s.id) AS enrollments,
                 (SELECT COUNT(*) FROM orders o WHERE o.student_id = s.id) AS orders
          FROM students s
          WHERE (@id IS NULL OR s.id = @id)
            AND (@search IS NULL OR s.full_name LIKE CONCAT('%', @search, '%') OR s.email LIKE CONCAT('%', @search, '%'))
          ORDER BY s.id", limit, ("@id", id), ("@search", search));

    [McpServerTool(Name = "get_student_progress", ReadOnly = true), Description("Per-course lesson completion for a student.")]
    public Task<List<Dictionary<string, object?>>> GetStudentProgress(
        [Description("Student id")] int studentId,
        [Description("Optional course id")] int? courseId = null) => db.QueryAsync(
        @"SELECT e.course_id, c.title, e.enrolled_at,
                 (SELECT COUNT(*) FROM lessons l WHERE l.course_id = e.course_id) AS total_lessons,
                 (SELECT COUNT(*) FROM lesson_progress p WHERE p.student_id = e.student_id AND p.course_id = e.course_id) AS completed_lessons,
                 ROUND(100 * (SELECT COUNT(*) FROM lesson_progress p WHERE p.student_id = e.student_id AND p.course_id = e.course_id)
                       / NULLIF((SELECT COUNT(*) FROM lessons l WHERE l.course_id = e.course_id), 0), 1) AS percent_complete,
                 (SELECT MAX(p.completed_at) FROM lesson_progress p WHERE p.student_id = e.student_id AND p.course_id = e.course_id) AS last_activity
          FROM enrollments e JOIN courses c ON c.id = e.course_id
          WHERE e.student_id = @sid AND (@cid IS NULL OR e.course_id = @cid)
          ORDER BY e.course_id", 100, ("@sid", studentId), ("@cid", courseId));

    [McpServerTool(Name = "list_at_risk_students", ReadOnly = true),
     Description("Enrolments with no lesson activity for N days (relative to LEARNHUB_NOW) and under 50% complete.")]
    public Task<List<Dictionary<string, object?>>> ListAtRiskStudents(
        [Description("Days of inactivity (default 30)")] int inactiveDays = 30,
        int limit = 100) => db.QueryAsync(
        @"SELECT s.id AS student_id, s.full_name, s.email, c.id AS course_id, c.title,
                 COUNT(DISTINCT p.lesson_id) AS completed_lessons,
                 (SELECT COUNT(*) FROM lessons l WHERE l.course_id = c.id) AS total_lessons,
                 COALESCE(MAX(p.completed_at), e.enrolled_at) AS last_activity
          FROM enrollments e
          JOIN students s ON s.id = e.student_id
          JOIN courses c ON c.id = e.course_id
          LEFT JOIN lesson_progress p ON p.student_id = e.student_id AND p.course_id = e.course_id
          GROUP BY e.id, s.id, c.id
          HAVING last_activity < DATE_SUB(@now, INTERVAL @days DAY)
             AND completed_lessons < 0.5 * total_lessons
          ORDER BY last_activity", limit, ("@now", Db.Now), ("@days", inactiveDays));
}
