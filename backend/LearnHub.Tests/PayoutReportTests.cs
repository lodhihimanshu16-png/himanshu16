namespace LearnHub.Tests;

public class PayoutReportTests
{
    [Fact]
    public void Payouts_AreSeventyPercentOfTheMonthsSales_PerInstructor()
    {
        using var env = new TestEnv();
        env.AddToCart("Asha", 2, 3, 4);                // Liam 34, Sofia 25.50, Ken 25.50 after the bundle
        env.Shop.Checkout("Asha", null);
        env.Clock.Now = new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc);
        env.AddToCart("Ben", 2);                        // bought in April
        env.Shop.Checkout("Ben", null);

        var march = env.Payouts.Build(2026, 3);

        Assert.Equal("2026-03", march.Month);
        Assert.Equal(new[] { "Ken Watanabe", "Liam Carter", "Sofia Alvarez" }, march.Rows.Select(r => r.Instructor));
        var liam = march.Rows.Single(r => r.Instructor == "Liam Carter");
        Assert.Equal((1, 34m, 23.80m), (liam.Lines, liam.Gross, liam.Share));
        Assert.Equal(85m, march.TotalGross);
        Assert.Equal(59.50m, march.TotalShare);

        var april = env.Payouts.Build(2026, 4);
        Assert.Equal(40m, Assert.Single(april.Rows).Gross);
        Assert.Empty(env.Payouts.Build(2026, 5).Rows);
    }

    [Fact]
    public void Payouts_RefundedOrder_IsExcludedFromInstructorRowAndTotals()
    {
        using var env = new TestEnv();                    // all orders created 2026-03-01 12:00 UTC
        Buy(env, "Asha", 2);                              // Liam's course 2, 40 paid
        var refunded = Buy(env, "Ben", 2);                // Liam's course 2, 40, refunded below
        Refund(env, refunded);

        var march = env.Payouts.Build(2026, 3);

        Assert.Equal(new[] { "Liam Carter" }, march.Rows.Select(r => r.Instructor));
        var liam = march.Rows.Single();
        // Paid only: 1 line, gross 40, share 40 x 0.70 = 28. Buggy SQL gives 2 lines, 80, 56.
        Assert.Equal((1, 40m, 28m), (liam.Lines, liam.Gross, liam.Share));
        Assert.Equal(40m, march.TotalGross);              // 40
        Assert.Equal(28m, march.TotalShare);              // 28
    }

    [Fact]
    public void Payouts_InstructorWithOnlyRefundedOrders_HasNoRow_OtherRowUnchanged()
    {
        using var env = new TestEnv();
        var refunded = Buy(env, "Ben", 2);                // Liam's course 2, 40, refunded below
        Buy(env, "Asha", 3);                              // Sofia's course 3, 30, paid
        Refund(env, refunded);

        var march = env.Payouts.Build(2026, 3);

        // Liam's only order was refunded, so no Liam row at all.
        Assert.Equal(new[] { "Sofia Alvarez" }, march.Rows.Select(r => r.Instructor));
        var sofia = march.Rows.Single();
        // 1 line, gross 30, share 30 x 0.70 = 21.
        Assert.Equal((1, 30m, 21m), (sofia.Lines, sofia.Gross, sofia.Share));
        Assert.Equal(30m, march.TotalGross);
        Assert.Equal(21m, march.TotalShare);
    }

    [Fact]
    public void Payouts_OrderBoughtInMarchAndRefundedInApril_IsExcludedFromMarchReport()
    {
        using var env = new TestEnv();
        env.Clock.Now = new DateTime(2026, 3, 28, 12, 0, 0, DateTimeKind.Utc);
        var order = Buy(env, "Asha", 2);                  // Liam's course 2, 40, created 28 March

        env.Clock.Now = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc);   // 6 days later, inside the 7-day window
        Refund(env, order);

        env.Clock.Now = new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc);
        Buy(env, "Ben", 2);                               // Liam's course 2, 40, paid, created in April

        // The March order is now Refunded, so the March report has no rows and no totals.
        var march = env.Payouts.Build(2026, 3);
        Assert.Empty(march.Rows);
        Assert.Equal(0m, march.TotalGross);
        Assert.Equal(0m, march.TotalShare);

        // April holds only Ben's paid order: 1 line, gross 40, share 28. Passes before and after the fix,
        // because the March order is dated in March.
        var april = env.Payouts.Build(2026, 4);
        Assert.Equal((1, 40m, 28m), (Assert.Single(april.Rows).Lines, april.Rows[0].Gross, april.Rows[0].Share));
    }

    [Fact]
    public void Payouts_TotalShare_MatchesSumOfInstructorRevenueShares_WithOneRefund()
    {
        using var env = new TestEnv();                    // all orders created 2026-03-01
        Buy(env, "Asha", 2, 3);                           // Liam 40 + Sofia 30, paid (2 lines, no bundle)
        Buy(env, "Ben", 4);                               // Ken 30, paid
        var refunded = Buy(env, "Chen", 2);               // Liam 40, refunded below
        Refund(env, refunded);

        var march = env.Payouts.Build(2026, 3);

        // Paid only: Ken 30 -> 21, Liam 40 -> 28, Sofia 30 -> 21. Total gross 100, total share 70.
        Assert.Equal(100m, march.TotalGross);
        Assert.Equal(70m, march.TotalShare);

        // Revenue per instructor: Liam gross 80 - refunds 40 = 40 -> 28; Sofia 21; Ken 21; others 0. Sum 70.
        var revenueShareSum = env.Shop.GetInstructorRevenue().Sum(r => r.InstructorShare);
        Assert.Equal(70m, revenueShareSum);
        Assert.Equal(revenueShareSum, march.TotalShare);  // exact: all shares are whole pence here
    }

    private static int Buy(TestEnv env, string name, params int[] courseIds)
    {
        env.AddToCart(name, courseIds);
        var (order, error) = env.Shop.Checkout(name, null);
        Assert.Null(error);
        return order!.Id;
    }

    private static void Refund(TestEnv env, int orderId)
    {
        var (_, error) = env.Shop.RefundOrder(orderId);
        Assert.Null(error);
    }
}
