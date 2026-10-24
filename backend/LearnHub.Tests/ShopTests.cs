using LearnHub.Api.Models;

namespace LearnHub.Tests;

// One test (or a small group) for each shop rule. The clock is fixed so dates never change.
// Prices: course 2 = 40, course 3 = 30, course 4 = 30, course 6 = 60. Courses 1 and 5 are free.
public class ShopTests
{
    private static readonly DateTime Today = TestData.Today;

    // R1
    [Fact]
    public void R1_EveryCourseHasAPrice_AndAtLeastTwoAreFree()
    {
        using var env = new TestEnv();
        var courses = env.Db.Courses.ToList();
        Assert.All(courses, c => Assert.True(c.Price >= 0));
        Assert.True(courses.Count(c => c.Price == 0) >= 2);
        Assert.True(courses.Count(c => c.Price > 0) >= 3);
    }

    // R2
    [Fact]
    public void R2_FreeCourseEnrollsDirectly_PaidCourseDoesNot()
    {
        using var env = new TestEnv();
        Assert.NotNull(env.Enrollments.EnrollFree(1, "Asha").Enrollment);
        var paid = env.Enrollments.EnrollFree(2, "Asha");
        Assert.Null(paid.Enrollment);
        Assert.Contains("paid", paid.Error);
    }

    // R3
    [Fact]
    public void R3_Cart_HoldsManyCourses_PerStudent_AndRemoves()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3);
        env.AddToCart("Ben", 4);

        Assert.Equal(new[] { 2, 3 }, env.Shop.GetCart("asha").Select(i => i.CourseId));
        Assert.Single(env.Shop.GetCart("Ben"));
        Assert.True(env.Shop.RemoveFromCart("ASHA", 2));
        Assert.False(env.Shop.RemoveFromCart("Asha", 2));
        Assert.Single(env.Shop.GetCart("Asha"));
    }

    // R4
    [Fact]
    public void R4_FreeCourse_CannotBeAddedToCart()
    {
        using var env = new TestEnv();
        var result = env.Shop.AddToCart("Asha", 1);
        Assert.Null(result.Item);
        Assert.Equal("This course is free. Enroll in it directly.", result.Error);
    }

    // R5
    [Fact]
    public void R5_OwnedCourse_AndDuplicate_AreRejected()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        Assert.Equal("Already in your cart.", env.Shop.AddToCart("ASHA", 2).Error);

        env.Shop.Checkout("Asha", null);
        Assert.Equal("You already own this course.", env.Shop.AddToCart("Asha", 2).Error);
    }

    // R6
    [Fact]
    public void R6_Bundle_GivesFifteenPercentOff_OnlyForThreeOrMoreCourses()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3);
        var two = env.Shop.Quote("Asha", null);
        Assert.Equal(0m, two.BundleDiscount);
        Assert.Equal(70m, two.Total);

        env.AddToCart("Asha", 4);
        var three = env.Shop.Quote("Asha", null);
        Assert.Equal(100m, three.Subtotal);
        Assert.Equal(15m, three.BundleDiscount);
        Assert.Equal(85m, three.Total);
        Assert.Equal(6m, three.Lines.First(l => l.CourseId == 2).BundleDiscount);
    }

    // R7
    [Fact]
    public void R7_Coupons_ArePercentOrFixed_AndCodeIgnoresCase()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        var percent = env.Shop.Quote("Asha", "welcome10");
        Assert.Equal(4m, percent.CouponDiscount);
        Assert.Equal(36m, percent.Total);

        env.AddCoupon("FIVE", "Fixed", 5, Today.AddDays(30), 10);
        var fixedOff = env.Shop.Quote("Asha", "Five");
        Assert.Equal(5m, fixedOff.CouponDiscount);
        Assert.Equal(35m, fixedOff.Total);
    }

    // R8
    [Fact]
    public void R8_Coupon_IsRejected_WhenUnknownExpiredOrUsedUp()
    {
        using var env = new TestEnv();
        env.AddCoupon("LASTDAY", "Percent", 10, Today.Date, 10);
        env.AddCoupon("FULL", "Percent", 10, Today.AddDays(5), 2, usedCount: 3);
        env.AddToCart("Asha", 2);

        var unknown = env.Shop.Quote("Asha", "NOPE");
        Assert.True(unknown.CouponRejected);
        Assert.Equal("Coupon not found.", unknown.CouponMessage);
        Assert.Equal("This coupon has expired.", env.Shop.Quote("Asha", "OLD5").CouponMessage);
        Assert.Equal("This coupon has no uses left.", env.Shop.Quote("Asha", "FULL").CouponMessage);
        Assert.False(env.Shop.Quote("Asha", "LASTDAY").CouponRejected); // still valid on its last day

        env.Clock.Now = Today.AddDays(1);
        Assert.Equal("This coupon has expired.", env.Shop.Quote("Asha", "LASTDAY").CouponMessage);
    }

    // R9
    [Fact]
    public void R9_Coupon_CanBeUsedOnlyOncePerStudent()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        Assert.NotNull(env.Shop.Checkout("Asha", "WELCOME10").Order);

        env.AddToCart("Asha", 3);
        var again = env.Shop.Quote("Asha", "WELCOME10");
        Assert.True(again.CouponRejected);
        Assert.Equal("You have already used this coupon.", again.CouponMessage);

        env.AddToCart("Ben", 3);
        Assert.False(env.Shop.Quote("Ben", "WELCOME10").CouponRejected);
        Assert.NotNull(env.Shop.Checkout("Ben", "WELCOME10").Order);
    }

    // R10
    [Fact]
    public void R10_BundleComesFirst_ThenCoupon_AndMinimumIsCheckedAfterBundle()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 4);
        var quote = env.Shop.Quote("Asha", "WELCOME10");
        Assert.Equal(100m, quote.Subtotal);
        Assert.Equal(15m, quote.BundleDiscount);
        Assert.Equal(8.50m, quote.CouponDiscount);
        Assert.Equal(76.50m, quote.Total);

        // 100 before the bundle, but only 85 after it. A minimum of 90 must fail.
        env.AddCoupon("MIN90", "Percent", 10, Today.AddDays(30), 10, minOrder: 90);
        Assert.True(env.Shop.Quote("Asha", "MIN90").CouponRejected);
        Assert.Contains("at least 90.00", env.Shop.Quote("Asha", "MIN90").CouponMessage);
    }

    // R11
    [Fact]
    public void R11_CategoryCoupon_OnlyDiscountsMatchingLines()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 6); // 40 + 30 + 60 = 130. Only course 6 is Cloud.
        var quote = env.Shop.Quote("Asha", "cloud25");

        Assert.Equal(19.50m, quote.BundleDiscount);
        Assert.Equal(0m, quote.Lines.First(l => l.CourseId == 2).CouponDiscount);
        Assert.Equal(12.75m, quote.Lines.First(l => l.CourseId == 6).CouponDiscount);
        Assert.Equal(97.75m, quote.Total);

        env.AddToCart("Ben", 2);
        var none = env.Shop.Quote("Ben", "CLOUD25");
        Assert.True(none.CouponRejected);
        Assert.Equal("This coupon only works for Cloud courses.", none.CouponMessage);
    }

    // R12
    [Fact]
    public void R12_Total_NeverGoesBelowZero()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 3); // 30, and FLAT50 takes 50 off
        var quote = env.Shop.Quote("Asha", "FLAT50");
        Assert.Equal(30m, quote.CouponDiscount);
        Assert.Equal(0m, quote.Total);
        Assert.All(quote.Lines, l => Assert.True(l.Final >= 0));

        var order = env.Shop.Checkout("Asha", "FLAT50").Order;
        Assert.Equal(0m, order!.Total);
        Assert.True(env.Enrollments.IsEnrolled("Asha", 3));
    }

    // R13
    [Fact]
    public void R13_Money_RoundsToTwoDecimals_HalfAwayFromZero()
    {
        using var env = new TestEnv();
        env.Db.Courses.Add(new Course
        {
            Id = 7, InstructorId = 1, Title = "Penny course", ShortDescription = "x", Category = "Test", Level = "Beginner",
            Price = 0.01m, CreatedAt = Today, UpdatedAt = Today,
            Lessons = { new Lesson { Id = 100, Position = 1, Title = "One", DurationMinutes = 5 } }
        });
        env.Db.SaveChanges();
        env.AddCoupon("HALF", "Percent", 50, Today.AddDays(30), 10);
        env.AddToCart("Asha", 7);

        var quote = env.Shop.Quote("Asha", "HALF"); // 0.005 rounds up to 0.01
        Assert.Equal(0.01m, quote.CouponDiscount);
        Assert.Equal(0m, quote.Total);

        env.AddToCart("Ben", 2, 3, 4);
        var ben = env.Shop.Quote("Ben", "WELCOME10"); // 3.4 + 2.55 + 2.55
        Assert.Equal(new[] { 3.40m, 2.55m, 2.55m }, ben.Lines.Select(l => l.CouponDiscount));
    }

    // R14
    [Fact]
    public void R14_Preview_ShowsEachLine_AndChangesNothing()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 4);
        var quote = env.Shop.Quote("Asha", "WELCOME10");

        Assert.Equal(3, quote.Lines.Count);
        var line = quote.Lines.First(l => l.CourseId == 2);
        Assert.Equal((40m, 6m, 3.40m, 30.60m), (line.Price, line.BundleDiscount, line.CouponDiscount, line.Final));
        Assert.Contains("WELCOME10", quote.CouponMessage);

        Assert.Equal(3, env.Shop.GetCart("Asha").Count);
        Assert.Empty(env.Db.Orders);
        Assert.Equal(0, env.CouponUses("WELCOME10"));
    }

    // R15
    [Fact]
    public void R15_Checkout_RejectsEmptyCart_BadCoupon_AndCoursesAlreadyOwned()
    {
        using var env = new TestEnv();
        Assert.Equal("Your cart is empty.", env.Shop.Checkout("Asha", null).Error);

        env.AddToCart("Asha", 2);
        Assert.Equal("Coupon not found.", env.Shop.Checkout("Asha", "NOPE").Error);
        Assert.Equal("This coupon has expired.", env.Shop.Checkout("Asha", "OLD5").Error);
        Assert.Empty(env.Db.Orders);

        env.Enrollments.Enroll(2, "Asha"); // enrolled some other way after the course went into the cart
        Assert.Contains("already own", env.Shop.Checkout("Asha", null).Error);
    }

    // R16
    [Fact]
    public void R16_Checkout_SavesOrder_Enrolls_EmptiesCart_AndCountsCouponUse()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 4);
        var (order, error) = env.Shop.Checkout("Asha", "welcome10");

        Assert.Null(error);
        Assert.Equal(1, order!.Id);
        Assert.Equal("Paid", order.Status);
        Assert.Equal(Today, order.CreatedAt);
        Assert.Equal("WELCOME10", order.CouponCode);
        Assert.Equal(76.50m, order.Total);
        Assert.Equal(3, order.Lines.Count);
        Assert.All(new[] { 2, 3, 4 }, id => Assert.True(env.Enrollments.IsEnrolled("Asha", id)));
        Assert.Empty(env.Shop.GetCart("Asha"));
        Assert.Equal(1, env.CouponUses("WELCOME10"));
    }

    // R17
    [Fact]
    public void R17_Orders_AreListedPerStudent()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        env.Shop.Checkout("Asha", null);
        env.AddToCart("Asha", 3);
        env.Shop.Checkout("Asha", null);
        env.AddToCart("Ben", 4);
        env.Shop.Checkout("Ben", null);

        Assert.Equal(new[] { 2, 1 }, env.Shop.GetOrders("asha").Select(o => o.Id));
        Assert.Single(env.Shop.GetOrders("Ben"));
        Assert.Empty(env.Shop.GetOrders("Zoe"));
    }

    // R18
    [Fact]
    public void R18_Refund_IsRefused_WhenProgressIs40Percent()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 4); // 5 lessons
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;
        env.CompleteLessons("Asha", 4, 2); // 40%

        var result = env.Shop.RefundOrder(orderId);
        Assert.Null(result.Order);
        Assert.Contains("40%", result.Error);
        Assert.Contains("Data Analysis with SQL", result.Error);
        Assert.Equal("Paid", env.Db.Orders.First().Status);
    }

    [Fact]
    public void R18_Refund_IsAllowedUpToSevenDays_ThenRefused()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;

        env.Clock.Now = Today.AddDays(7);
        env.AddToCart("Ben", 3);
        var late = env.Shop.Checkout("Ben", null).Order!.Id; // bought on day 7
        Assert.NotNull(env.Shop.RefundOrder(orderId).Order); // exactly 7 days is still fine

        env.Clock.Now = Today.AddDays(14).AddHours(1);
        Assert.Contains("within 7 days", env.Shop.RefundOrder(late).Error);
    }

    [Fact]
    public void R18_Refund_Twice_IsAConflict_AndUnknownOrderIsNotFound()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;
        env.Shop.RefundOrder(orderId);

        Assert.Equal("This order was already refunded.", env.Shop.RefundOrder(orderId).Error);
        Assert.Equal("Order not found.", env.Shop.RefundOrder(999).Error);
    }

    // R18 regression: the 7-day window is measured from Order.CreatedAt, never from a touched UpdatedAt.
    [Fact]
    public void R18_Refund_WindowIgnoresUpdatedAt_SoATouchedOrderCannotBeRefundedLate()
    {
        using var env = new TestEnv();
        env.Clock.Now = Today.AddDays(-8);
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id; // created 8 days ago

        env.Clock.Now = Today;
        var order = env.Db.Orders.First(o => o.Id == orderId);
        order.UpdatedAt = Today; // simulate a recently touched order
        env.Db.SaveChanges();

        var result = env.Shop.RefundOrder(orderId);
        Assert.Null(result.Order);
        Assert.Contains("within 7 days", result.Error);
        Assert.Equal("Paid", env.Db.Orders.First(o => o.Id == orderId).Status);
    }

    // R18: a multi-course order is refused when any one course is at 30% or more.
    [Fact]
    public void R18_Refund_MultiCourseOrder_IsRefused_WhenOneCourseIsAtThirtyPercent()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 4);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;
        env.CompleteLessons("Asha", 4, 2); // 40% of course 4
        // course 2 stays at 0%

        var result = env.Shop.RefundOrder(orderId);
        Assert.Null(result.Order);
        Assert.Contains("Data Analysis with SQL", result.Error);
        Assert.Equal("Paid", env.Db.Orders.First(o => o.Id == orderId).Status);
    }

    // R18: a refused refund changes nothing at all.
    [Fact]
    public void R18_RefusedRefund_ChangesNothing()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 4, 2);
        var orderId = env.Shop.Checkout("Asha", "WELCOME10").Order!.Id;
        env.CompleteLessons("Asha", 4, 2); // 40%, so refused
        var studentId = env.Students.Find("Asha")!.Id;
        var enrollmentsBefore = env.Db.Enrollments.Count(e => e.StudentId == studentId);
        var progressBefore = env.Db.LessonProgress.Count(p => p.StudentId == studentId);
        var usesBefore = env.CouponUses("WELCOME10");
        Assert.Equal(1, usesBefore);

        Assert.NotNull(env.Shop.RefundOrder(orderId).Error);

        Assert.Equal("Paid", env.Db.Orders.First(o => o.Id == orderId).Status);
        Assert.Empty(env.Db.Refunds);
        Assert.Equal(enrollmentsBefore, env.Db.Enrollments.Count(e => e.StudentId == studentId));
        Assert.Equal(progressBefore, env.Db.LessonProgress.Count(p => p.StudentId == studentId));
        Assert.Equal(2, env.Db.Enrollments.Count(e => e.StudentId == studentId && (e.CourseId == 2 || e.CourseId == 4)));
        Assert.Equal(usesBefore, env.CouponUses("WELCOME10"));
    }

    // R18: exactly 7 days old with progress under 30% is allowed.
    [Fact]
    public void R18_Refund_ExactlySevenDaysOld_WithProgressUnderThirty_IsAllowed()
    {
        using var env = new TestEnv();
        env.Clock.Now = Today.AddDays(-7);
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;
        env.CompleteLessons("Asha", 2, 1); // 25% of course 2

        env.Clock.Now = Today;
        var (order, error) = env.Shop.RefundOrder(orderId);

        Assert.Null(error);
        Assert.Equal("Refunded", order!.Status);
    }

    // R19
    [Fact]
    public void R19_Refund_RemovesEnrollmentAndProgress_AndGivesBackTheCouponUse()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 4, 3, 2);
        var orderId = env.Shop.Checkout("Asha", "WELCOME10").Order!.Id;
        env.CompleteLessons("Asha", 4, 1); // 20%, under the limit

        var (order, error) = env.Shop.RefundOrder(orderId);

        Assert.Null(error);
        Assert.Equal("Refunded", order!.Status);
        Assert.Empty(env.Db.Enrollments.Where(e => e.Student.FullName == "Asha"));
        Assert.Empty(env.Db.LessonProgress.Where(p => p.StudentId == env.Students.Find("Asha")!.Id));
        Assert.Equal(0, env.CouponUses("WELCOME10"));
        Assert.Single(env.Db.Refunds);

        // The student can use the coupon and buy the course again.
        env.AddToCart("Asha", 4);
        Assert.NotNull(env.Shop.Checkout("Asha", "WELCOME10").Order);
    }

    [Fact]
    public void R19_EnrollmentIds_StayUnique_AfterARefund()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", null).Order!.Id;
        env.Enrollments.Enroll(1, "Ben");
        env.Shop.RefundOrder(orderId);
        env.Enrollments.Enroll(5, "Cara");

        var ids = env.Db.Enrollments.Select(e => e.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    // R20
    [Fact]
    public void R20_InstructorRevenue_IsSeventyPercentOfNetSales()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 4);               // Liam 40, Sofia 30, Ken 30 -> 34, 25.50, 25.50 after bundle
        env.Shop.Checkout("Asha", null);
        env.AddToCart("Ben", 2);                       // Liam 40
        var benOrder = env.Shop.Checkout("Ben", null).Order!.Id;
        env.Shop.RefundOrder(benOrder);                // Ben's 40 comes back

        var revenue = env.Shop.GetInstructorRevenue();
        var liam = revenue.First(r => r.Instructor == "Liam Carter");
        Assert.Equal(74m, liam.GrossSales);               // 34 + 40
        Assert.Equal(40m, liam.Refunds);
        Assert.Equal(23.80m, liam.InstructorShare);       // 70% of 34
        Assert.Equal(17.85m, revenue.First(r => r.Instructor == "Sofia Alvarez").InstructorShare); // 70% of 25.50
        Assert.Equal(0m, revenue.First(r => r.Instructor == "Maya Rao").GrossSales);
    }

    // R21a: a coupon with one use can be used once. The next student is refused at preview and at checkout.
    [Fact]
    public void R21a_OneUseCoupon_CanBeUsedOnce_ThenNobodyElseCanUseIt()
    {
        using var env = new TestEnv();
        env.AddCoupon("ONCE", "Percent", 10, Today.AddDays(30), 1);
        env.AddToCart("Asha", 2);
        env.AddToCart("Ben", 3);

        Assert.NotNull(env.Shop.Checkout("Asha", "ONCE").Order);

        var quote = env.Shop.Quote("Ben", "ONCE");
        Assert.True(quote.CouponRejected);
        Assert.Equal("This coupon has no uses left.", quote.CouponMessage);

        var (order, error) = env.Shop.Checkout("Ben", "ONCE");
        Assert.Null(order);
        Assert.Equal("This coupon has no uses left.", error);
        Assert.Single(env.Db.Orders);
    }

    // R21b: a coupon at its limit is refused, and a coupon one use short of its limit still works.
    [Fact]
    public void R21b_Coupon_AtItsLimit_IsRefused_ButOneUseShortStillWorks()
    {
        using var env = new TestEnv();
        env.AddCoupon("FULLTWO", "Percent", 10, Today.AddDays(30), 2, usedCount: 2);
        env.AddCoupon("LASTONE", "Percent", 10, Today.AddDays(30), 2, usedCount: 1);
        env.AddToCart("Asha", 2);

        Assert.True(env.Shop.Quote("Asha", "FULLTWO").CouponRejected);
        var (order, error) = env.Shop.Checkout("Asha", "FULLTWO");
        Assert.Null(order);
        Assert.Equal("This coupon has no uses left.", error);
        Assert.Empty(env.Db.Orders);
        Assert.Single(env.Shop.GetCart("Asha"));
        Assert.Equal(2, env.CouponUses("FULLTWO"));

        Assert.False(env.Shop.Quote("Asha", "LASTONE").CouponRejected);
        Assert.NotNull(env.Shop.Checkout("Asha", "LASTONE").Order);
        Assert.Equal(2, env.CouponUses("LASTONE"));
    }

    // R21c: paid orders count too, so a counter that says "no uses" cannot reopen a used-up coupon.
    [Fact]
    public void R21c_PaidOrders_CountEvenIfTheCounterSaysNoUsesLeft()
    {
        using var env = new TestEnv();
        env.AddCoupon("ONCE", "Percent", 10, Today.AddDays(30), 1);
        env.AddToCart("Asha", 2);
        Assert.NotNull(env.Shop.Checkout("Asha", "ONCE").Order);

        var coupon = env.Db.Coupons.First(c => c.Code == "ONCE");
        coupon.UsedCount = 0; // the counter is out of step with the paid orders
        env.Db.SaveChanges();

        env.AddToCart("Ben", 3);
        Assert.True(env.Shop.Quote("Ben", "ONCE").CouponRejected);
        Assert.Null(env.Shop.Checkout("Ben", "ONCE").Order);
        Assert.Single(env.Db.Orders);
    }

    // R21d: a refund gives the coupon slot back, and the next student can take it.
    [Fact]
    public void R21d_RefundGivesTheCouponSlotBack_ForTheNextStudent()
    {
        using var env = new TestEnv();
        env.AddCoupon("ONCE", "Percent", 10, Today.AddDays(30), 1);
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", "ONCE").Order!.Id;
        Assert.NotNull(env.Shop.RefundOrder(orderId).Order);

        env.AddToCart("Ben", 3);
        env.AddToCart("Cara", 4);
        Assert.NotNull(env.Shop.Checkout("Ben", "ONCE").Order);

        // Ben's order now uses the single slot, so Cara is refused.
        Assert.Equal("This coupon has no uses left.", env.Shop.Checkout("Cara", "ONCE").Error);
        Assert.Single(env.Shop.GetCart("Cara"));
    }

    // R21e: the once-per-student rule holds at checkout, even when there was no preview first.
    [Fact]
    public void R21e_Checkout_RefusesASecondUseOfTheCoupon_WithoutAPreview()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        Assert.NotNull(env.Shop.Checkout("Asha", "WELCOME10").Order);

        env.AddToCart("Asha", 3);
        var (order, error) = env.Shop.Checkout("Asha", "WELCOME10");
        Assert.Null(order);
        Assert.Equal("You have already used this coupon.", error);

        // The code is matched without regard to case, so this is refused too.
        (order, error) = env.Shop.Checkout("Asha", "welcome10");
        Assert.Null(order);
        Assert.Equal("You have already used this coupon.", error);

        Assert.Single(env.Db.Orders);
        Assert.Equal(new[] { 3 }, env.Shop.GetCart("Asha").Select(i => i.CourseId));
        Assert.Equal(1, env.CouponUses("WELCOME10"));
    }

    // R21f: a refunded order does not stop the same student using the coupon again.
    [Fact]
    public void R21f_RefundedOrder_DoesNotStopTheSameStudentUsingTheCouponAgain()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2);
        var orderId = env.Shop.Checkout("Asha", "WELCOME10").Order!.Id;
        Assert.NotNull(env.Shop.RefundOrder(orderId).Order);

        Assert.False(env.Shop.HasUsedCoupon("Asha", "WELCOME10"));
        env.AddToCart("Asha", 3);
        Assert.False(env.Shop.Quote("Asha", "WELCOME10").CouponRejected);
        Assert.NotNull(env.Shop.Checkout("Asha", "WELCOME10").Order);
    }
}
