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
            Id = 2, Price = 40m, Title = "JavaScript for the Web", ShortDescription = "Build interactive web pages with modern JavaScript.",
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
            Id = 3, Price = 30m, Title = "UI Design Basics", ShortDescription = "Learn layout, colour and type for clean interfaces.",
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
            Id = 4, Price = 30m, Title = "Data Analysis with SQL", ShortDescription = "Ask questions of data using SQL queries.",
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
            Id = 6, Price = 60m, Title = "Deploying Apps with Containers", ShortDescription = "Package an app in a container and run it anywhere.",
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

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public List<Enrollment> Enrollments { get; } = new();
    public List<CartItem> Cart { get; } = new();
    public List<Order> Orders { get; } = new();
    public List<Coupon> Coupons { get; } = new()
    {
        new Coupon { Code = "WELCOME10", Type = "Percent", Value = 10 },
        new Coupon { Code = "SAVE20", Type = "Fixed", Value = 20, MinOrder = 50m },
        new Coupon { Code = "CLOUD25", Type = "Percent", Value = 25, Category = "Cloud" },
        new Coupon { Code = "FLAT50", Type = "Fixed", Value = 50 },
        new Coupon { Code = "OLD5", Type = "Percent", Value = 5, ExpiresOn = new DateTime(2020, 1, 1) },
    };
    private int _nextEnrollmentId = 1;
    private int _nextOrderId = 1;
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

        if (Courses.First(c => c.Id == courseId).Price > 0)
            return (null, "This course is paid. Add it to the cart and check out.");

        return (AddEnrollment(courseId, name), null);
    }

    private Enrollment AddEnrollment(int courseId, string name)
    {
        var enrollment = new Enrollment { Id = _nextEnrollmentId++, CourseId = courseId, StudentName = name, EnrolledAt = Clock() };
        Enrollments.Add(enrollment);
        return enrollment;
    }

    /// <summary>Joins a free course. A paid course is refused.</summary>
    public (Enrollment? Enrollment, string? Error) EnrollFree(int courseId, string? studentName) => Enroll(courseId, studentName);

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

    // ---- Cart ----

    private static bool SameName(string a, string? b) =>
        string.Equals(a, b?.Trim(), StringComparison.OrdinalIgnoreCase);

    public List<(CartItem Item, Course Course)> GetCart(string? studentName)
    {
        var name = studentName?.Trim() ?? "";
        return Cart.Where(i => SameName(i.StudentName, name))
            .Select(i => (i, Courses.First(c => c.Id == i.CourseId))).ToList();
    }

    /// <summary>Returns the cart item, or an error text (required, unknown, free, owned, already in cart).</summary>
    public (CartItem? Item, string? Error) AddToCart(string? studentName, int courseId)
    {
        var (item, _, error) = AddToCartChecked(studentName, courseId);
        return (item, error);
    }

    /// <summary>Like AddToCart, but also returns an error kind: "BadRequest" or "Conflict".</summary>
    public (CartItem? Item, string? ErrorKind, string? Error) AddToCartChecked(string? studentName, int courseId)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "BadRequest", "studentName is required.");
        var course = Courses.FirstOrDefault(c => c.Id == courseId);
        if (course is null) return (null, "BadRequest", "Course not found.");
        if (course.Price <= 0) return (null, "BadRequest", "Free courses cannot be added to the cart. Enroll directly instead.");
        if (IsEnrolled(name, courseId)) return (null, "Conflict", "You already own this course.");
        if (Cart.Any(i => i.CourseId == courseId && SameName(i.StudentName, name)))
            return (null, "Conflict", "This course is already in your cart.");

        var item = new CartItem { StudentName = name, CourseId = courseId, AddedAt = Clock() };
        Cart.Add(item);
        return (item, null, null);
    }

    public bool RemoveFromCart(string? studentName, int courseId)
    {
        var name = studentName?.Trim() ?? "";
        return Cart.RemoveAll(i => i.CourseId == courseId && SameName(i.StudentName, name)) > 0;
    }

    // ---- Coupons and pricing ----

    private static decimal Money(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    /// <summary>Counts paid (not refunded) orders that used the coupon, optionally for one student.</summary>
    public int CouponUses(string code, string? student = null) =>
        Orders.Count(o => o.Status == "Paid" && string.Equals(o.CouponCode, code, StringComparison.OrdinalIgnoreCase) &&
            (student is null || SameName(o.StudentName, student)));

    /// <summary>Checks unknown, expired, no uses left, and once per student. Returns an error text or null.</summary>
    public string? ValidateCoupon(string? code, string? studentName, out Coupon? coupon)
    {
        var c = code?.Trim() ?? "";
        coupon = Coupons.FirstOrDefault(x => string.Equals(x.Code, c, StringComparison.OrdinalIgnoreCase));
        if (coupon is null) return "Coupon code not found.";
        if (Clock().Date > coupon.ExpiresOn.Date) return "This coupon has expired.";
        if (coupon.MaxUses is int max && CouponUses(coupon.Code) >= max) return "This coupon has no uses left.";
        if (CouponUses(coupon.Code, studentName ?? "") > 0) return "You have already used this coupon.";
        return null;
    }

    /// <summary>Prices the student's cart: bundle first, then the optional coupon. Changes nothing.</summary>
    public PriceQuote Quote(string? studentName, string? couponCode)
    {
        var name = studentName?.Trim() ?? "";
        var quote = new PriceQuote();
        var courses = GetCart(name).Select(x => x.Course).ToList();
        var bundle = courses.Count >= 3;
        foreach (var c in courses)
        {
            var bd = bundle ? Money(c.Price * 0.15m) : 0m;
            quote.Lines.Add(new OrderLine
            {
                CourseId = c.Id, Title = c.Title, Instructor = c.Instructor, Price = c.Price,
                BundleDiscount = bd, Final = c.Price - bd
            });
        }
        quote.Subtotal = quote.Lines.Sum(l => l.Price);
        quote.BundleDiscount = quote.Lines.Sum(l => l.BundleDiscount);
        quote.Total = quote.Subtotal - quote.BundleDiscount;

        if (string.IsNullOrWhiteSpace(couponCode)) return quote;

        var error = ValidateCoupon(couponCode, name, out var coupon);
        if (error is null && quote.Lines.Count == 0) error = "Your cart is empty.";
        if (error is null && quote.Total < coupon!.MinOrder)
            error = $"This coupon needs an order of at least {coupon.MinOrder:0.00}.";

        var applicable = new List<OrderLine>();
        if (error is null)
        {
            applicable = quote.Lines.Where(l => coupon!.Category == "" || string.Equals(
                courses.First(c => c.Id == l.CourseId).Category, coupon.Category, StringComparison.OrdinalIgnoreCase)).ToList();
            if (applicable.Count == 0) error = $"This coupon only applies to {coupon!.Category} courses.";
        }
        if (error is not null)
        {
            quote.CouponMessage = error;
            return quote;
        }

        if (coupon!.Type.Equals("Percent", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var l in applicable) l.CouponDiscount = Money(l.Final * coupon.Value / 100m);
        }
        else
        {
            var pool = applicable.Sum(l => l.Final);
            var total = Math.Min(coupon.Value, pool);
            var left = total;
            for (var i = 0; i < applicable.Count; i++)
            {
                var l = applicable[i];
                var share = i == applicable.Count - 1 ? left : Math.Min(left, Money(total * l.Final / pool));
                l.CouponDiscount = share;
                left -= share;
            }
        }
        foreach (var l in applicable)
        {
            l.CouponDiscount = Math.Min(l.CouponDiscount, l.Final);
            l.Final -= l.CouponDiscount;
        }
        quote.CouponDiscount = quote.Lines.Sum(l => l.CouponDiscount);
        quote.Total = Math.Max(0m, quote.Lines.Sum(l => l.Final));
        quote.Coupon = coupon;
        quote.CouponMessage = "Coupon applied.";
        return quote;
    }

    // ---- Checkout and orders ----

    public (Order? Order, string? Error) Checkout(string? studentName, string? couponCode)
    {
        var name = studentName?.Trim();
        if (string.IsNullOrEmpty(name)) return (null, "studentName is required.");
        var cart = GetCart(name);
        if (cart.Count == 0) return (null, "Your cart is empty.");
        var owned = cart.FirstOrDefault(x => IsEnrolled(name, x.Course.Id));
        if (owned.Course is not null) return (null, $"You already own {owned.Course.Title}. Remove it from the cart.");

        var quote = Quote(name, couponCode);
        if (!string.IsNullOrWhiteSpace(couponCode) && quote.Coupon is null) return (null, quote.CouponMessage);

        var order = new Order
        {
            Id = _nextOrderId++, StudentName = name, CreatedAt = Clock(), Lines = quote.Lines,
            CouponCode = quote.Coupon?.Code, Subtotal = quote.Subtotal, BundleDiscount = quote.BundleDiscount,
            CouponDiscount = quote.CouponDiscount, Total = quote.Total, Status = "Paid"
        };
        Orders.Add(order);
        foreach (var l in order.Lines) AddEnrollment(l.CourseId, name);
        Cart.RemoveAll(i => SameName(i.StudentName, name));
        return (order, null);
    }

    /// <summary>The student's orders, newest first.</summary>
    public List<Order> GetOrders(string? studentName) =>
        Orders.Where(o => SameName(o.StudentName, studentName)).OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id).ToList();

    /// <summary>When Order is null, ErrorKind is "notfound", "conflict" or "invalid".</summary>
    public (Order? Order, string? ErrorKind, string? Error) RefundOrder(int orderId)
    {
        var order = Orders.FirstOrDefault(o => o.Id == orderId);
        if (order is null) return (null, "notfound", "Order not found.");
        if (order.Status == "Refunded") return (null, "conflict", "This order has already been refunded.");
        if (Clock() - order.CreatedAt > TimeSpan.FromDays(7))
            return (null, "invalid", "Refunds are only possible within 7 days of purchase.");

        foreach (var line in order.Lines)
        {
            var course = Courses.First(c => c.Id == line.CourseId);
            var total = course.Lessons.Count;
            var done = course.Lessons.Count(l => Progress.Any(p => p.CourseId == course.Id && p.LessonId == l.Id && SameName(p.StudentName, order.StudentName)));
            if (total > 0 && done * 100m / total >= 30m)
                return (null, "invalid", $"You have finished {Math.Round(done * 100m / total, MidpointRounding.AwayFromZero):0}% of {course.Title}. Refunds need progress under 30% on every course.");
        }

        order.Status = "Refunded";
        foreach (var line in order.Lines)
        {
            Enrollments.RemoveAll(e => e.CourseId == line.CourseId && SameName(e.StudentName, order.StudentName));
            Progress.RemoveAll(p => p.CourseId == line.CourseId && SameName(p.StudentName, order.StudentName));
        }
        return (order, null, null);
    }

    public List<(string Instructor, decimal GrossSales, decimal Refunds, decimal InstructorShare)> GetInstructorRevenue() =>
        Courses.Select(c => c.Instructor).Distinct().Select(ins =>
        {
            var lines = Orders.SelectMany(o => o.Lines.Select(l => (o, l))).Where(x => string.Equals(x.l.Instructor, ins, StringComparison.OrdinalIgnoreCase)).ToList();
            var gross = lines.Sum(x => x.l.Final);
            var refunds = lines.Where(x => x.o.Status == "Refunded").Sum(x => x.l.Final);
            return (ins, Money(gross), Money(refunds), Money((gross - refunds) * 0.7m));
        }).ToList();
}
