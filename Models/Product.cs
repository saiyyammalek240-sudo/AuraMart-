using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkEasePro.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    [Display(Name = "Product Title")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000000)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Sale Price (₹)")]
    public decimal Price { get; set; }

    [Range(1, 1000000)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Original / MRP (₹)")]
    public decimal OriginalPrice { get; set; }

    [Required]
    [StringLength(300)]
    [Display(Name = "Short Overview")]
    public string ShortDescription { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Detailed Description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0, 10000)]
    [Display(Name = "Units in Stock")]
    public int StockQuantity { get; set; } = 25;

    [Required]
    [StringLength(400)]
    [Display(Name = "Primary Image URL")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsFeatured { get; set; } = false;
    public bool IsTrending { get; set; } = false;
    public bool IsOnSale { get; set; } = false;

    public double Rating { get; set; } = 4.8;
    public int ReviewCount { get; set; } = 12;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [NotMapped]
    public int DiscountPercent => OriginalPrice > Price && OriginalPrice > 0 
        ? (int)Math.Round(((OriginalPrice - Price) / OriginalPrice) * 100) 
        : 0;

    [NotMapped]
    public bool InStock => StockQuantity > 0;
}
