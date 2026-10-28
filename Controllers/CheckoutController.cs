using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;
using ParkEasePro.Models.ViewModels;

namespace ParkEasePro.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<CartViewModel> GetUserCartAsync(string userId)
    {
        var items = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        decimal discount = 0.00m;
        string? coupon = HttpContext.Session.GetString("AppliedCoupon");
        if (!string.IsNullOrEmpty(coupon))
        {
            var c = await _context.Coupons.FirstOrDefaultAsync(cp => cp.Code == coupon && cp.IsActive);
            if (c != null && items.Sum(i => i.SubTotal) >= c.MinOrderAmount)
            {
                discount = Math.Round(items.Sum(i => i.SubTotal) * (c.DiscountPercent / 100.0m), 2);
            }
        }

        return new CartViewModel
        {
            Items = items,
            DiscountAmount = discount,
            AppliedCouponCode = coupon
        };
    }

    // GET: /Checkout
    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await GetUserCartAsync(userId);

        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";
            return RedirectToAction("Index", "Shop");
        }

        var user = await _userManager.FindByIdAsync(userId);

        var model = new CheckoutViewModel
        {
            FullName = user?.FullName ?? string.Empty,
            Phone = user?.PhoneNumber ?? string.Empty,
            Address = user?.Address ?? string.Empty,
            City = user?.City ?? string.Empty,
            PostalCode = user?.PostalCode ?? string.Empty,
            Cart = cart
        };

        return View(model);
    }

    // POST: /Checkout/PlaceOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await GetUserCartAsync(userId);

        if (!cart.Items.Any())
        {
            TempData["ErrorMessage"] = "Your shopping cart is empty.";
            return RedirectToAction("Index", "Shop");
        }

        // Validate stock availability
        foreach (var item in cart.Items)
        {
            if (item.Product.StockQuantity < item.Quantity)
            {
                ModelState.AddModelError("", $"Sorry, \"{item.Product.Name}\" only has {item.Product.StockQuantity} units in stock.");
                model.Cart = cart;
                return View("Index", model);
            }
        }

        // Generate Order Ref & Simulated TXN
        string orderRef = $"AUR-{DateTime.UtcNow.Year}-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
        string txnId = $"{model.SelectedPaymentMethod}-TXN-" + Guid.NewGuid().ToString("N")[..8].ToUpper();

        var order = new Order
        {
            OrderNumber = orderRef,
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            SubTotal = cart.SubTotal,
            DiscountAmount = cart.DiscountAmount,
            ShippingFee = cart.ShippingFee,
            GrandTotal = cart.GrandTotal,
            Status = OrderStatus.Processing,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = model.SelectedPaymentMethod,
            TransactionId = txnId,
            ShippingFullName = model.FullName,
            ShippingPhone = model.Phone,
            ShippingAddress = model.Address,
            ShippingCity = model.City,
            ShippingPostalCode = model.PostalCode,
            TrackingNumber = "AUR-TRK-" + Guid.NewGuid().ToString("N")[..8].ToUpper(),
            EstimatedDelivery = DateTime.UtcNow.AddDays(3),
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.ImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList()
        };

        // Decrement product inventory
        foreach (var item in cart.Items)
        {
            item.Product.StockQuantity -= item.Quantity;
        }

        // Empty user cart
        _context.CartItems.RemoveRange(cart.Items);
        HttpContext.Session.Remove("AppliedCoupon");

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Thank you! Your order {order.OrderNumber} has been successfully placed.";
        return RedirectToAction(nameof(Confirmation), new { id = order.Id });
    }

    // GET: /Checkout/Confirmation/5
    public async Task<IActionResult> Confirmation(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
