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
}
