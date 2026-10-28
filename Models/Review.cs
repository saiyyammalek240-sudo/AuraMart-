using System.ComponentModel.DataAnnotations;

namespace ParkEasePro.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    [Range(1, 5)]
    [Display(Name = "Rating Stars")]
    public int Rating { get; set; } = 5;

    [Required]
    [StringLength(500)]
    [Display(Name = "Your Review")]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Coupon
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty; // e.g. "SAVE20", "FESTIVE10"

    [Range(1, 90)]
    public int DiscountPercent { get; set; } = 15;

    [Range(0, 100000)]
    public decimal MinOrderAmount { get; set; } = 500;

    public bool IsActive { get; set; } = true;
}
