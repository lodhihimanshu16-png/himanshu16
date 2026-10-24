using System.ComponentModel;
using ModelContextProtocol.Server;

namespace LearnHub.Mcp.Tools;

[McpServerToolType]
public sealed class ShopTools(Db db)
{
    private const string RefundSql =
        @"SELECT r.id AS refund_id, r.order_id, o.student_id, r.amount, r.reason, o.created_at AS ordered_at, r.refunded_at,
                 TIMESTAMPDIFF(HOUR, o.created_at, r.refunded_at) / 24 AS days_to_refund,
                 (SELECT ROUND(MAX(100.0 * (SELECT COUNT(*) FROM lesson_progress p
                                              WHERE p.course_id = ol.course_id AND p.student_id = o.student_id AND p.completed_at <= r.refunded_at)
                                         / NULLIF((SELECT COUNT(*) FROM lessons l WHERE l.course_id = ol.course_id), 0)), 1)
                    FROM order_lines ol WHERE ol.order_id = o.id)
                   AS percent_complete
          FROM refunds r JOIN orders o ON o.id = r.order_id";

    private const string CouponSql =
        @"SELECT c.code, c.discount_type, c.discount_value, c.expires_on, c.max_uses, c.used_count,
                 COUNT(o.id) AS paid_orders, COUNT(DISTINCT o.student_id) AS distinct_students
          FROM coupons c LEFT JOIN orders o ON o.coupon_code = c.code AND o.status = 'Paid'
          GROUP BY c.id";

    [McpServerTool(Name = "list_orders", ReadOnly = true), Description("List orders, optionally filtered by student, status (Paid/Refunded) and date range (yyyy-MM-dd).")]
    public Task<List<Dictionary<string, object?>>> ListOrders(
        int? studentId = null, string? status = null, string? from = null, string? to = null, int limit = 100) => db.QueryAsync(
        @"SELECT id, student_id, coupon_code, subtotal, bundle_discount, coupon_discount, total, status, created_at
          FROM orders
          WHERE (@sid IS NULL OR student_id = @sid) AND (@status IS NULL OR status = @status)
            AND (@from IS NULL OR created_at >= @from) AND (@to IS NULL OR created_at < DATE_ADD(@to, INTERVAL 1 DAY))
          ORDER BY created_at DESC", limit, ("@sid", studentId), ("@status", status), ("@from", from), ("@to", to));

    [McpServerTool(Name = "get_order", ReadOnly = true), Description("Get an order with its lines and any refund.")]
    public Task<List<Dictionary<string, object?>>> GetOrder(int id) => db.QueryAsync(
        @"SELECT o.id AS order_id, o.student_id, o.coupon_code, o.total, o.status, o.created_at,
                 l.course_id, c.title, l.price, l.final_price, r.amount AS refund_amount, r.refunded_at, r.reason
          FROM orders o
          JOIN order_lines l ON l.order_id = o.id
          JOIN courses c ON c.id = l.course_id
          LEFT JOIN refunds r ON r.order_id = o.id
          WHERE o.id = @id", 100, ("@id", id));

    [McpServerTool(Name = "list_refunds", ReadOnly = true), Description("List refunds with order date, days to refund and progress in the refunded courses.")]
    public Task<List<Dictionary<string, object?>>> ListRefunds(string? from = null, string? to = null, int limit = 100) => db.QueryAsync(
        RefundSql + " WHERE (@from IS NULL OR r.refunded_at >= @from) AND (@to IS NULL OR r.refunded_at < DATE_ADD(@to, INTERVAL 1 DAY)) ORDER BY r.refunded_at DESC",
        limit, ("@from", from), ("@to", to));

    [McpServerTool(Name = "find_refund_policy_violations", ReadOnly = true),
     Description("Refunds issued more than 7 days after purchase, or when progress in the order's courses was 30% or more.")]
    public Task<List<Dictionary<string, object?>>> FindRefundViolations(int limit = 200) => db.QueryAsync(
        "SELECT * FROM (" + RefundSql + ") x WHERE x.refunded_at > DATE_ADD(x.ordered_at, INTERVAL 7 DAY) OR x.percent_complete >= 30 ORDER BY x.refund_id",
        limit);

    [McpServerTool(Name = "get_coupon_usage", ReadOnly = true), Description("Coupon stats: recorded used_count vs actual paid orders, distinct students and max_uses.")]
    public Task<List<Dictionary<string, object?>>> GetCouponUsage(string? code = null, int limit = 100) => db.QueryAsync(
        "SELECT * FROM (" + CouponSql + ") x WHERE (@code IS NULL OR code = @code) ORDER BY code", limit, ("@code", code));

    [McpServerTool(Name = "find_coupon_overuse", ReadOnly = true), Description("Coupons used beyond max_uses, or by the same student more than once (paid orders only).")]
    public Task<List<Dictionary<string, object?>>> FindCouponOveruse(int limit = 100) => db.QueryAsync(
        "SELECT * FROM (" + CouponSql + ") x WHERE paid_orders > max_uses OR paid_orders > distinct_students ORDER BY code", limit);

    [McpServerTool(Name = "instructor_payout", ReadOnly = true),
     Description("Instructor payout at 70% of net sales (paid order lines only, refunded orders excluded). Optional instructor id and month (yyyy-MM).")]
    public Task<List<Dictionary<string, object?>>> InstructorPayout(int? instructorId = null, string? month = null, int limit = 100) => db.QueryAsync(
        @"SELECT i.id AS instructor_id, i.full_name, COUNT(l.id) AS lines_sold,
                 ROUND(SUM(l.final_price), 2) AS net_sales, ROUND(SUM(l.final_price) * 0.70, 2) AS payout
          FROM order_lines l
          JOIN orders o ON o.id = l.order_id AND o.status = 'Paid'
          JOIN courses c ON c.id = l.course_id
          JOIN instructors i ON i.id = c.instructor_id
          WHERE (@iid IS NULL OR i.id = @iid) AND (@month IS NULL OR DATE_FORMAT(o.created_at, '%Y-%m') = @month)
          GROUP BY i.id ORDER BY i.id", limit, ("@iid", instructorId), ("@month", month));
}
