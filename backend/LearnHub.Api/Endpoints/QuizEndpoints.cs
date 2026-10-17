using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public record QuizAnswer(int QuestionId, int SelectedIndex);
public record QuizSubmitRequest(string? StudentName, List<QuizAnswer>? Answers);

public static class QuizEndpoints
{
    public static void MapQuizEndpoints(this WebApplication app)
    {
        app.MapGet("/api/lessons/{lessonId:int}/quiz", (int lessonId, InMemoryStore store) =>
        {
            var questions = store.GetQuiz(lessonId);
            if (questions.Count == 0) return Results.NotFound();

            // correctIndex is left out on purpose.
            return Results.Ok(questions.Select(q => new { q.Id, q.LessonId, q.Question, q.Options }));
        });

        app.MapPost("/api/lessons/{lessonId:int}/quiz/submit", (int lessonId, QuizSubmitRequest request, InMemoryStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.StudentName))
                return Results.BadRequest(new { error = "studentName is required." });
            if (request.Answers is null || request.Answers.Count == 0)
                return Results.BadRequest(new { error = "answers are required." });
            if (store.GetQuiz(lessonId).Count == 0) return Results.NotFound();

            var result = store.ScoreQuiz(lessonId, request.Answers.Select(a => (a.QuestionId, a.SelectedIndex)));
            return Results.Ok(new { score = result.Score, total = result.Total, passed = result.Passed });
        });
    }
}
