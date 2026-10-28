using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkEasePro.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string OrderNumber { get; set; } = string.Empty; // e.g. AUR-2026-X8921

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0.00m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0.00m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; } = 0.00m;

    [Column(TypeName = "decimal(18,2)")]
    public decimal GrandTotal { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Placed;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Paid;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.UPI;

    [StringLength(60)]
    public string? TransactionId { get; set; }

    // Shipping Snapshot
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Recipient Name")]
    public string ShippingFullName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Contact Phone")]
    public string ShippingPhone { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    [Display(Name = "Delivery Street Address")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    [Display(Name = "City")]
    public string ShippingCity { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Postal / PIN Code")]
    public string ShippingPostalCode { get; set; } = string.Empty;

    [StringLength(50)]
    public string? TrackingNumber { get; set; }

    public DateTime? EstimatedDelivery { get; set; }

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
