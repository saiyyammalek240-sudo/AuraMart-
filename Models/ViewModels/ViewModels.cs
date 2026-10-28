using System.ComponentModel.DataAnnotations;

namespace ParkEasePro.Models.ViewModels;

public class HomeViewModel
{
    public List<Category> Categories { get; set; } = new();
    public List<Product> FeaturedProducts { get; set; } = new();
    public List<Product> TrendingProducts { get; set; } = new();
    public List<Product> OnSaleProducts { get; set; } = new();
}

public class ShopCatalogViewModel
{
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    public int? SelectedCategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; } // "price_asc", "price_desc", "newest", "rating"

    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; }
}

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.SubTotal);
    public decimal DiscountAmount { get; set; } = 0.00m;
    public decimal ShippingFee => SubTotal > 999 || SubTotal == 0 ? 0.00m : 99.00m;
    public decimal GrandTotal => Math.Max(0, SubTotal - DiscountAmount + ShippingFee);
    public string? AppliedCouponCode { get; set; }
    public int TotalQuantity => Items.Sum(i => i.Quantity);
}

public class CheckoutViewModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Full Recipient Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [Display(Name = "Contact Phone Number")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    [Display(Name = "Shipping Street Address")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [Display(Name = "PIN / Postal Code")]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Method")]
    public PaymentMethod SelectedPaymentMethod { get; set; } = PaymentMethod.UPI;

    // Simulated Gateway Card fields
    [Display(Name = "Cardholder Name")]
    public string? CardName { get; set; }

    [Display(Name = "Card Number")]
    public string? CardNumber { get; set; }

    [Display(Name = "Expiry MM/YY")]
    public string? CardExpiry { get; set; }

    [Display(Name = "CVV")]
    public string? CardCvv { get; set; }

    // Simulated UPI
    [Display(Name = "UPI ID / VPA")]
    public string? UpiId { get; set; }

    public CartViewModel Cart { get; set; } = new();
}

public class AdminEcommerceDashboardViewModel
{
    public decimal TotalSalesRevenue { get; set; }
    public decimal MonthSalesRevenue { get; set; }
    public int TotalOrdersCount { get; set; }
    public int TotalProductsCount { get; set; }
    public int LowStockProductsCount { get; set; }
    public int ActiveCustomersCount { get; set; }

    public List<Order> RecentOrders { get; set; } = new();
    public List<Product> LowStockProducts { get; set; } = new();

    // Chart.js Datasets
    public List<string> RevenueDates { get; set; } = new();
    public List<decimal> DailyRevenues { get; set; } = new();
    public List<string> CategoryLabels { get; set; } = new();
    public List<int> CategoryCounts { get; set; } = new();
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number (Optional)")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Delivery Address (Optional)")]
    public string? Address { get; set; }

    [Display(Name = "City (Optional)")]
    public string? City { get; set; }

    [Display(Name = "PIN Code (Optional)")]
    public string? PostalCode { get; set; }

    public string? ReturnUrl { get; set; }
}

public class GoogleAuthSimulationViewModel
{
    [Required(ErrorMessage = "Google Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string FullName { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
