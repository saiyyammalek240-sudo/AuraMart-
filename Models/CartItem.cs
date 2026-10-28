using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkEasePro.Models;

public class CartItem
{
    public int Id { get; set; }

    public string? UserId { get; set; }
    public virtual ApplicationUser? User { get; set; }

    [StringLength(100)]
    public string? SessionCartId { get; set; } // Supports guest checkout before login!

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public decimal SubTotal => UnitPrice * Quantity;
}
