namespace LearnHub.Api.Models;

public class Coupon
{
    public string Code { get; set; } = "";
    /// <summary>"Percent" or "Fixed".</summary>
    public string Type { get; set; } = "Percent";
    public decimal Value { get; set; }
    /// <summary>Empty means every category.</summary>
    public string Category { get; set; } = "";
    public decimal MinOrder { get; set; }
    /// <summary>Last day the coupon works (the whole day counts).</summary>
    public DateTime ExpiresOn { get; set; } = new DateTime(2099, 12, 31);
    /// <summary>Total uses allowed. Null means no limit.</summary>
    public int? MaxUses { get; set; }
}
