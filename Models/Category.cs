using System.ComponentModel.DataAnnotations;

namespace ParkEasePro.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Icon Class (e.g. bi-laptop)")]
    public string IconClass { get; set; } = "bi-box-seam";

    [StringLength(300)]
    [Display(Name = "Image URL")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
