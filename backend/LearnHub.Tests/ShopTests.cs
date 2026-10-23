using LearnHub.Api.Data;
using LearnHub.Api.Models;

namespace LearnHub.Tests;

public class ShopTests
{
    private static readonly DateTime Today = new(2026, 3, 10, 12, 0, 0);

    private static InMemoryStore NewStore()
    {
        var store = new InMemoryStore();
        store.Clock = () => Today;
        return store;
    }

    private static void Fill(InMemoryStore s, string name, params int[] ids)
    {
        foreach (var id in ids)
            Assert.Null(s.AddToCart(name, id).Error);
    }

    private static Order Buy(InMemoryStore s, string name, string? code, params int[] ids)
    {
        Fill(s, name, ids);
        var r = s.Checkout(name, code);
        Assert.Null(r.Error);
        return r.Order!;
    }

    // R1
    [Fact]
    public void R1_Prices_AreNotNegative_AndAtLeastTwoFree()
    {
        var s = NewStore();
        Assert.All(s.Courses, c => Assert.True(c.Price >= 0));
        Assert.True(s.Courses.Count(c => c.Price == 0) >= 2);
    }

    // R2
    [Fact]
    public void R2_FreeCourse_JoinsDirectly_PaidCourseRefused()
    {
        var s = NewStore();
        Assert.NotNull(s.Enroll(1, "Asha").Enrollment);
        var paid = s.Enroll(2, "Asha");
        Assert.Null(paid.Enrollment);
        Assert.False(string.IsNullOrEmpty(paid.Error));
    }

    // R3
    [Fact]
    public void R3_EachStudentHasOwnCart_AddAndRemove()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3);
        Fill(s, "Ben", 4);
        Assert.Equal(2, s.GetCart("Asha").Count);
        Assert.Single(s.GetCart("Ben"));
        Assert.True(s.RemoveFromCart("Asha", 2));
        Assert.False(s.RemoveFromCart("Asha", 2));
        Assert.Single(s.GetCart("Asha"));
    }

    // R4
    [Fact]
    public void R4_FreeCourse_CannotBeAddedToCart()
    {
        var s = NewStore();
        var r = s.AddToCart("Asha", 1);
        Assert.Null(r.Item);
        Assert.Contains("Free", r.Error);
    }

    // R5
    [Fact]
    public void R5_OwnedOrAlreadyInCart_Refused()
    {
        var s = NewStore();
        Fill(s, "Asha", 2);
        Assert.NotNull(s.AddToCart("Asha", 2).Error);
        Buy(s, "Asha", null);
        s.Cart.Clear();
        Assert.Contains("own", s.AddToCart("Asha", 2).Error);
    }

    // R6
    [Fact]
    public void R6_Bundle_FifteenPercentOnThreeOrMore_NoneOnTwo()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3);
        Assert.Equal(0m, s.Quote("Asha", null).BundleDiscount);
        Fill(s, "Asha", 4);
        var q = s.Quote("Asha", null);
        Assert.Equal(15.00m, q.BundleDiscount);
        Assert.Equal(85.00m, q.Total);
        Assert.Equal(6.00m, q.Lines.First(l => l.CourseId == 2).BundleDiscount);
    }

    // R7
    [Fact]
    public void R7_Coupons_PercentFixed_CaseInsensitive_CategoryOnly()
    {
        var s = NewStore();
        Fill(s, "Asha", 2);
        Assert.Equal(4.00m, s.Quote("Asha", "welcome10").CouponDiscount);
        Assert.Equal(40.00m - 4.00m, s.Quote("Asha", "WeLcOmE10").Total);
        s.Coupons.Add(new Coupon { Code = "FIX5", Type = "Fixed", Value = 5, ExpiresOn = new DateTime(2099, 12, 31) });
        Assert.Equal(35.00m, s.Quote("Asha", "fix5").Total);
        Fill(s, "Asha", 6);
        var q = s.Quote("Asha", "CLOUD25");
        Assert.Equal(15.00m, q.CouponDiscount);
        Assert.Equal(0m, q.Lines.First(l => l.CourseId == 2).CouponDiscount);
    }

    // R8
    [Fact]
    public void R8_Coupon_Refused_Unknown_Expired_NoUsesLeft_LastDayWorks()
    {
        var s = NewStore();
        Fill(s, "Asha", 2);
        Assert.NotNull(s.ValidateCoupon("NOPE", "Asha", out _));
        Assert.NotNull(s.ValidateCoupon("OLD5", "Asha", out _));
        s.Coupons.Add(new Coupon { Code = "NONE", Type = "Percent", Value = 10, MaxUses = 0, ExpiresOn = new DateTime(2099, 12, 31) });
        Assert.Contains("no uses left", s.ValidateCoupon("NONE", "Asha", out _));
        s.Coupons.Add(new Coupon { Code = "LAST", Type = "Percent", Value = 10, ExpiresOn = Today.Date });
        s.Clock = () => Today.Date.AddHours(23).AddMinutes(59);
        Assert.Null(s.ValidateCoupon("LAST", "Asha", out _));
        s.Clock = () => Today.Date.AddDays(1);
        Assert.NotNull(s.ValidateCoupon("LAST", "Asha", out _));
    }

    // R9
    [Fact]
    public void R9_SameCouponOncePerStudent_RefundGivesUseBack()
    {
        var s = NewStore();
        var order = Buy(s, "Asha", "WELCOME10", 2);
        Assert.NotNull(s.ValidateCoupon("WELCOME10", "Asha", out _));
        Assert.Null(s.ValidateCoupon("WELCOME10", "Ben", out _));
        Assert.Null(s.RefundOrder(order.Id).Error);
        Assert.Null(s.ValidateCoupon("WELCOME10", "Asha", out _));
    }

    // R10
    [Fact]
    public void R10_BundleThenCoupon_76_50_AndMinimumAfterBundle()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3, 4);
        var q = s.Quote("Asha", "WELCOME10");
        Assert.Equal(8.50m, q.CouponDiscount);
        Assert.Equal(76.50m, q.Total);
        Assert.Equal(3.40m, q.Lines.First(l => l.CourseId == 2).CouponDiscount);
        Assert.Equal(2.55m, q.Lines.First(l => l.CourseId == 3).CouponDiscount);
        Assert.Equal(2.55m, q.Lines.First(l => l.CourseId == 4).CouponDiscount);

        s.Coupons.Add(new Coupon { Code = "MIN90", Type = "Fixed", Value = 10, MinOrder = 90m, ExpiresOn = new DateTime(2099, 12, 31) });
        var m = s.Quote("Asha", "MIN90");
        Assert.Null(m.Coupon);
        Assert.Equal(0m, m.CouponDiscount);
        Assert.Equal(85.00m, m.Total);
        Assert.Contains("90.00", m.CouponMessage);
    }

    // R11
    [Fact]
    public void R11_CategoryCoupon_Example97_75_AndRefusedWithoutMatchingLine()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3, 6);
        var q = s.Quote("Asha", "CLOUD25");
        Assert.Equal(12.75m, q.CouponDiscount);
        Assert.Equal(97.75m, q.Total);

        Fill(s, "Ben", 2, 3);
        var r = s.Quote("Ben", "CLOUD25");
        Assert.Null(r.Coupon);
        Assert.False(string.IsNullOrEmpty(r.CouponMessage));
        Assert.Equal(70.00m, r.Total);
    }

    // R12
    [Fact]
    public void R12_TotalNeverBelowZero_Flat50On30_IsZero_AndSave20Refused()
    {
        var s = NewStore();
        Fill(s, "Asha", 3);
        var q = s.Quote("Asha", "FLAT50");
        Assert.Equal(30.00m, q.CouponDiscount);
        Assert.Equal(0.00m, q.Total);
        var order = s.Checkout("Asha", "FLAT50").Order!;
        Assert.Equal(0.00m, order.Total);
        Assert.True(s.IsEnrolled("Asha", 3));

        Fill(s, "Ben", 2);
        var r = s.Quote("Ben", "SAVE20");
        Assert.Null(r.Coupon);
        Assert.Contains("50.00", r.CouponMessage);
        Assert.Equal(40.00m, r.Total);
    }

    // R13
    [Fact]
    public void R13_HalfRoundsUp()
    {
        var s = NewStore();
        s.Courses.Add(new Course { Id = 99, Title = "Tiny", Price = 0.05m, Category = "Misc", Instructor = "Zed" });
        Fill(s, "Asha", 99);
        var q = s.Quote("Asha", "WELCOME10");
        Assert.Equal(0.01m, q.CouponDiscount);
        Assert.Equal(0.04m, q.Total);
    }

    // R14
    [Fact]
    public void R14_Preview_ShowsLinesAndTotals_ChangesNothing()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3);
        var q = s.Quote("Asha", "WELCOME10");
        Assert.Equal(2, q.Lines.Count);
        Assert.Equal(70.00m, q.Subtotal);
        Assert.Equal(63.00m, q.Total);
        Assert.Equal(2, s.GetCart("Asha").Count);
        Assert.Empty(s.Orders);
        Assert.Empty(s.Enrollments);
        Assert.Null(s.ValidateCoupon("WELCOME10", "Asha", out _));
    }

    // R15
    [Fact]
    public void R15_Checkout_RechecksEverything()
    {
        var s = NewStore();
        Assert.NotNull(s.Checkout("Asha", null).Error);
        Fill(s, "Asha", 2);
        Assert.NotNull(s.Checkout("Asha", "NOPE").Error);
        Assert.Empty(s.Orders);
        s.Enrollments.Add(new Enrollment { Id = 500, CourseId = 2, StudentName = "Asha" });
        var r = s.Checkout("Asha", null);
        Assert.Null(r.Order);
        Assert.Contains("already own", r.Error);
    }

    // R16
    [Fact]
    public void R16_GoodCheckout_SavesOrder_CountsCoupon_Enrolls_EmptiesCart()
    {
        var s = NewStore();
        Fill(s, "Asha", 2, 3, 4);
        var r = s.Checkout("Asha", "WELCOME10");
        Assert.Null(r.Error);
        Assert.Equal(76.50m, r.Order!.Total);
        Assert.Equal("Paid", r.Order.Status);
        Assert.Single(s.Orders);
        Assert.Equal(1, s.CouponUses("WELCOME10"));
        Assert.True(s.IsEnrolled("Asha", 2) && s.IsEnrolled("Asha", 3) && s.IsEnrolled("Asha", 4));
        Assert.Empty(s.GetCart("Asha"));
    }

    // R17
    [Fact]
    public void R17_Orders_OwnOnly_NewestFirst()
    {
        var s = NewStore();
        var a = Buy(s, "Asha", null, 2);
        s.Clock = () => Today.AddHours(1);
        var b = Buy(s, "Asha", null, 3);
        Buy(s, "Ben", null, 4);
        var list = s.GetOrders("Asha");
        Assert.Equal(new[] { b.Id, a.Id }, list.Select(o => o.Id).ToArray());
    }

    // R18
    [Fact]
    public void R18_Refund_Rules_40PercentMessage_Old_Second_Unknown()
    {
        var s = NewStore();
        var o = Buy(s, "Asha", null, 4);
        s.MarkLessonComplete("Asha", 4, 12);
        s.MarkLessonComplete("Asha", 4, 13);
        var r = s.RefundOrder(o.Id);
        Assert.Equal("invalid", r.ErrorKind);
        Assert.Equal("You have finished 40% of Data Analysis with SQL. Refunds need progress under 30% on every course.", r.Error);

        var s2 = NewStore();
        var o2 = Buy(s2, "Asha", null, 2);
        s2.Clock = () => Today.AddDays(8);
        Assert.Equal("invalid", s2.RefundOrder(o2.Id).ErrorKind);
        s2.Clock = () => Today.AddDays(7);
        Assert.Null(s2.RefundOrder(o2.Id).Error);
        Assert.Equal("conflict", s2.RefundOrder(o2.Id).ErrorKind);
        Assert.Equal("notfound", s2.RefundOrder(999).ErrorKind);
    }

    // R19
    [Fact]
    public void R19_Refund_SetsRefunded_RemovesEnrollmentsAndProgress_GivesCouponBack()
    {
        var s = NewStore();
        var o = Buy(s, "Asha", "WELCOME10", 2, 3, 4);
        s.MarkLessonComplete("Asha", 4, 12);
        s.Clock = () => Today.AddDays(1);
        var r = s.RefundOrder(o.Id);
        Assert.Null(r.Error);
        Assert.Equal("Refunded", r.Order!.Status);
        Assert.Empty(s.Enrollments);
        Assert.Empty(s.Progress);
        Assert.Equal(0, s.CouponUses("WELCOME10"));
        Assert.Null(s.ValidateCoupon("WELCOME10", "Asha", out _));
    }

    // R20
    [Fact]
    public void R20_InstructorRevenue_Liam_30_60_21_42_ThenZeroAfterRefund()
    {
        var s = NewStore();
        var o = Buy(s, "Asha", "WELCOME10", 2, 3, 4);
        var liam = s.GetInstructorRevenue().Single(x => x.Instructor == "Liam Carter");
        Assert.Equal(30.60m, liam.GrossSales);
        Assert.Equal(0m, liam.Refunds);
        Assert.Equal(21.42m, liam.InstructorShare);
        Assert.Equal(6, s.GetInstructorRevenue().Count);

        s.Clock = () => Today.AddDays(2);
        Assert.Null(s.RefundOrder(o.Id).Error);
        liam = s.GetInstructorRevenue().Single(x => x.Instructor == "Liam Carter");
        Assert.Equal(30.60m, liam.GrossSales);
        Assert.Equal(30.60m, liam.Refunds);
        Assert.Equal(0m, liam.InstructorShare);
    }
}
