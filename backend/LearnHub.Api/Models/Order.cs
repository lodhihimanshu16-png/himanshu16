namespace LearnHub.Api.Models;

public class OrderLine
{
    public int CourseId { get; set; }
    public string Title { get; set; } = "";
    public string Instructor { get; set; } = "";
    public decimal Price { get; set; }
    public decimal BundleDiscount { get; set; }
    public decimal CouponDiscount { get; set; }
    public decimal Final { get; set; }
}

public class PriceQuote
{
    public List<OrderLine> Lines { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal BundleDiscount { get; set; }
    public decimal CouponDiscount { get; set; }
    public decimal Total { get; set; }
    public string CouponMessage { get; set; } = "";
    /// <summary>The coupon that was applied, or null.</summary>
    public Coupon? Coupon { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string StudentName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderLine> Lines { get; set; } = new();
    public string? CouponCode { get; set; }
    public decimal Subtotal { get; set; }
    public decimal BundleDiscount { get; set; }
    public decimal CouponDiscount { get; set; }
    public decimal Total { get; set; }
    /// <summary>"Paid" or "Refunded".</summary>
    public string Status { get; set; } = "Paid";
}
