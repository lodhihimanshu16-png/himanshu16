namespace LearnHub.Api.Services;

/// <summary>
/// The refund rules. A refund is allowed only within RefundDays of the order being created
/// (exactly RefundDays is still allowed) and only when progress is below MaxProgressPercent
/// on every course in the order (MaxProgressPercent itself is refused).
/// </summary>
public static class RefundPolicy
{
    public const int RefundDays = 7;
    public const double MaxProgressPercent = 30;

    /// <summary>Returns the refusal message, or null when the refund is allowed.</summary>
    public static string? Evaluate(DateTime createdAt, DateTime refundedAt, IEnumerable<(string CourseTitle, double Percent)> courses)
    {
        if (refundedAt - createdAt > TimeSpan.FromDays(RefundDays))
            return $"Refunds are only possible within {RefundDays} days of the order.";

        foreach (var (title, percent) in courses)
        {
            if (percent >= MaxProgressPercent)
                return $"You have finished {Math.Round(percent)}% of {title}. Refunds need progress under {MaxProgressPercent}% on every course.";
        }

        return null;
    }
}
