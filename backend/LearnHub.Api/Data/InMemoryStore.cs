using LearnHub.Api.Models;

namespace LearnHub.Api.Data;

public class InMemoryStore
{
    public List<Course> Courses { get; } = new()
    {
        new Course
        {
            Id = 1, Title = "Intro to C# Programming", ShortDescription = "Learn the basics of C# by writing small programs.",
            Category = "Programming", Instructor = "Maya Rao", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 1, Title = "Setting up your tools", DurationMinutes = 10, Content = "Install the .NET SDK and run your first program." },
                new Lesson { Id = 2, Title = "Variables and types", DurationMinutes = 15, Content = "Store numbers, text and true/false values." },
                new Lesson { Id = 3, Title = "Conditions and loops", DurationMinutes = 20, Content = "Make decisions and repeat work with if and for." },
                new Lesson { Id = 4, Title = "Methods", DurationMinutes = 15, Content = "Group code into small reusable pieces." },
            }
        },
        new Course
        {
            Id = 2, Title = "JavaScript for the Web", ShortDescription = "Build interactive web pages with modern JavaScript.",
            Category = "Programming", Instructor = "Liam Carter", Level = "Intermediate",
            Lessons =
            {
                new Lesson { Id = 5, Title = "The DOM", DurationMinutes = 15, Content = "Find and change things on a web page." },
                new Lesson { Id = 6, Title = "Events", DurationMinutes = 12, Content = "React to clicks and key presses." },
                new Lesson { Id = 7, Title = "Fetching data", DurationMinutes = 18, Content = "Call an API with fetch and show the result." },
                new Lesson { Id = 8, Title = "Async code", DurationMinutes = 20, Content = "Understand promises and async/await." },
            }
        },
        new Course
        {
            Id = 3, Title = "UI Design Basics", ShortDescription = "Learn layout, colour and type for clean interfaces.",
            Category = "Design", Instructor = "Sofia Alvarez", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 9, Title = "Layout and spacing", DurationMinutes = 12, Content = "Use a grid and consistent spacing." },
                new Lesson { Id = 10, Title = "Colour", DurationMinutes = 10, Content = "Pick an accent colour and keep contrast high." },
                new Lesson { Id = 11, Title = "Typography", DurationMinutes = 12, Content = "Choose readable fonts and sizes." },
            }
        },
        new Course
        {
            Id = 4, Title = "Data Analysis with SQL", ShortDescription = "Ask questions of data using SQL queries.",
            Category = "Data", Instructor = "Ken Watanabe", Level = "Intermediate",
            Lessons =
            {
                new Lesson { Id = 12, Title = "SELECT basics", DurationMinutes = 15, Content = "Read rows from a table." },
                new Lesson { Id = 13, Title = "Filtering and sorting", DurationMinutes = 15, Content = "Use WHERE and ORDER BY." },
                new Lesson { Id = 14, Title = "Joins", DurationMinutes = 20, Content = "Combine data from two tables." },
                new Lesson { Id = 15, Title = "Grouping", DurationMinutes = 18, Content = "Count and total with GROUP BY." },
                new Lesson { Id = 16, Title = "Query practice", DurationMinutes = 25, Content = "Solve small real-world questions." },
            }
        },
        new Course
        {
            Id = 5, Title = "Cloud Fundamentals", ShortDescription = "Understand servers, storage and networks in the cloud.",
            Category = "Cloud", Instructor = "Priya Nair", Level = "Beginner",
            Lessons =
            {
                new Lesson { Id = 17, Title = "What is the cloud?", DurationMinutes = 10, Content = "Compute, storage and networking in plain words." },
                new Lesson { Id = 18, Title = "Virtual machines", DurationMinutes = 15, Content = "Start a server and connect to it." },
                new Lesson { Id = 19, Title = "Storage", DurationMinutes = 12, Content = "Files, blocks and databases." },
                new Lesson { Id = 20, Title = "Cost basics", DurationMinutes = 10, Content = "Understand what you pay for." },
            }
        },
        new Course
        {
            Id = 6, Title = "Deploying Apps with Containers", ShortDescription = "Package an app in a container and run it anywhere.",
            Category = "Cloud", Instructor = "Omar Hassan", Level = "Advanced",
            Lessons =
            {
                new Lesson { Id = 21, Title = "Why containers?", DurationMinutes = 10, Content = "The problem containers solve." },
                new Lesson { Id = 22, Title = "Writing a Dockerfile", DurationMinutes = 20, Content = "Describe how to build your app image." },
                new Lesson { Id = 23, Title = "Running containers", DurationMinutes = 15, Content = "Start, stop and inspect containers." },
                new Lesson { Id = 24, Title = "Compose", DurationMinutes = 20, Content = "Run several containers together." },
            }
        },
    };

    public List<Enrollment> Enrollments { get; } = new();
    public List<LessonProgress> Progress { get; } = new();

    public List<QuizQuestion> QuizQuestions { get; } = new()
    {
        Q(1, 1, "Which tool do you install to run C# programs?", 0, "The .NET SDK", "Photoshop", "A web browser", "Excel"),
        Q(2, 1, "What does 'dotnet run' do?", 1, "Deletes your project", "Builds and starts your program", "Installs Windows", "Opens a website"),
        Q(3, 2, "Which type stores whole numbers?", 2, "string", "bool", "int", "char"),
        Q(4, 2, "Which type stores true or false?", 0, "bool", "int", "double", "string"),
        Q(5, 3, "Which keyword repeats code a set number of times?", 1, "if", "for", "class", "using"),
        Q(6, 3, "What does 'if' do?", 1, "Repeats code", "Makes a decision", "Names a method", "Imports a library"),
        Q(7, 4, "Why use methods?", 1, "To make code slower", "To group code into reusable pieces", "To delete variables", "To change colours"),
        Q(8, 4, "Which word means a method returns nothing?", 2, "null", "empty", "void", "none"),
        Q(9, 5, "What does DOM stand for?", 0, "Document Object Model", "Data Output Mode", "Digital Order Map", "Display Object Menu"),
        Q(10, 5, "Which call finds an element by its id?", 0, "document.getElementById", "document.makeId", "window.find", "page.select"),
        Q(11, 6, "Which event fires when a button is clicked?", 1, "hover", "click", "load", "scroll"),
        Q(12, 6, "What does addEventListener do?", 0, "Runs code when an event happens", "Adds a CSS class", "Deletes an element", "Starts a server"),
        Q(13, 7, "Which function calls an API from the browser?", 0, "fetch", "send", "pull", "load"),
        Q(14, 7, "What format do most APIs return?", 0, "JSON", "MP3", "PDF", "ZIP"),
        Q(15, 8, "What does 'await' do?", 1, "Stops the browser", "Waits for a promise to finish", "Deletes a promise", "Repeats a loop"),
        Q(16, 8, "Which keyword marks a function that can use await?", 0, "async", "defer", "later", "wait"),
        Q(17, 9, "What helps keep a layout tidy?", 1, "Random spacing", "A grid and consistent spacing", "Many fonts", "Tiny text"),
        Q(18, 9, "What is white space good for?", 1, "Wasting room", "Making content easier to read", "Hiding content", "Slowing the page"),
        Q(19, 10, "What should you keep high for text colour?", 0, "Contrast", "Noise", "Saturation only", "Opacity only"),
        Q(20, 10, "How many accent colours should a simple design start with?", 0, "One", "Ten", "Twenty", "None"),
        Q(21, 11, "What makes text readable?", 0, "Clear fonts and good size", "Very small text", "Many styles at once", "Light grey on white"),
        Q(22, 11, "Which is a good body text size on the web?", 1, "4px", "16px", "60px", "2px"),
    };

    private static QuizQuestion Q(int id, int lessonId, string question, int correctIndex, params string[] options) =>
        new() { Id = id, LessonId = lessonId, Question = question, CorrectIndex = correctIndex, Options = options.ToList() };

    // ---- Courses ----

    public IEnumerable<Course> SearchCourses(string? search) =>
        Courses.Where(c => string.IsNullOrWhiteSpace(search) ||
            c.Title.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase));

    // ---- Enrollment ----

    public bool IsEnrolled(string studentName, int courseId) =>
        Enrollments.Any(e => e.CourseId == courseId &&
            string.Equals(e.StudentName, studentName.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns the new enrollment, or an error text ("required", "course", "duplicate").</summary>
    public (Enrollment? Enrollment, string? Error) Enroll(int courseId, string? studentName)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        if (!Courses.Any(c => c.Id == courseId)) return (null, "Course not found.");
        if (IsEnrolled(name, courseId)) return (null, "Already enrolled in this course.");

        var enrollment = new Enrollment { Id = Enrollments.Count + 1, CourseId = courseId, StudentName = name };
        Enrollments.Add(enrollment);
        return (enrollment, null);
    }

    // ---- Quiz ----

    public List<QuizQuestion> GetQuiz(int lessonId) =>
        QuizQuestions.Where(q => q.LessonId == lessonId).ToList();

    public (int Score, int Total, bool Passed) ScoreQuiz(int lessonId, IEnumerable<(int QuestionId, int SelectedIndex)> answers)
    {
        var questions = GetQuiz(lessonId);
        var score = questions.Count(q => answers.Any(a => a.QuestionId == q.Id && a.SelectedIndex == q.CorrectIndex));
        var total = questions.Count;
        return (score, total, total > 0 && score * 2 >= total);
    }

    // ---- Progress ----

    /// <summary>Marks a lesson done. Safe to call twice. Returns an error text or null.</summary>
    public string? MarkLessonComplete(string? studentName, int courseId, int lessonId)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return "studentName is required.";
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null) return "Course not found.";
        if (!course.Lessons.Any(l => l.Id == lessonId)) return "Lesson not found in this course.";
        if (!IsEnrolled(name, courseId)) return "Student is not enrolled in this course.";

        var done = Progress.Any(p => p.CourseId == courseId && p.LessonId == lessonId &&
            string.Equals(p.StudentName, name, StringComparison.OrdinalIgnoreCase));
        if (!done)
            Progress.Add(new LessonProgress { StudentName = name, CourseId = courseId, LessonId = lessonId });
        return null;
    }

    public List<(int CourseId, string CourseTitle, int CompletedLessons, int TotalLessons, int Percent, List<int> CompletedLessonIds)> GetProgress(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Enrollments
            .Where(e => string.Equals(e.StudentName, name, StringComparison.OrdinalIgnoreCase))
            .Select(e =>
            {
                var course = Courses.First(c => c.Id == e.CourseId);
                var ids = Progress
                    .Where(p => p.CourseId == course.Id && string.Equals(p.StudentName, name, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.LessonId).ToList();
                var total = course.Lessons.Count;
                var percent = total == 0 ? 0 : (int)Math.Round(100.0 * ids.Count / total);
                return (course.Id, course.Title, ids.Count, total, percent, ids);
            }).ToList();
    }
}
