using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;
using ParkEasePro.Models.ViewModels;

namespace ParkEasePro.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string CartSessionCookieKey = "AuraMart_CartId";

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetOrCreateCartSessionId()
    {
        if (Request.Cookies.TryGetValue(CartSessionCookieKey, out var cartId) && !string.IsNullOrEmpty(cartId))
        {
            return cartId;
        }

        string newCartId = Guid.NewGuid().ToString("N");
        Response.Cookies.Append(CartSessionCookieKey, newCartId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            IsEssential = true,
            HttpOnly = true
        });
        return newCartId;
    }

    private async Task<List<CartItem>> GetCurrentCartItemsAsync()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string sessionCartId = GetOrCreateCartSessionId();

        if (!string.IsNullOrEmpty(userId))
        {
            // If user logged in, associate any orphaned guest items with this user
            var guestItems = await _context.CartItems.Where(c => c.SessionCartId == sessionCartId && c.UserId == null).ToListAsync();
            foreach (var item in guestItems)
            {
                var existingUserItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == item.ProductId);
                if (existingUserItem != null)
                {
                    existingUserItem.Quantity += item.Quantity;
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.UserId = userId;
                }
            }
            if (guestItems.Any()) await _context.SaveChangesAsync();

            return await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        return await _context.CartItems
            .Include(c => c.Product)
                .ThenInclude(p => p.Category)
            .Where(c => c.SessionCartId == sessionCartId)
            .ToListAsync();
    }

    // GET: /Cart
    public async Task<IActionResult> Index()
    {
        var items = await GetCurrentCartItemsAsync();
        decimal discount = 0.00m;
        string? coupon = HttpContext.Session.GetString("AppliedCoupon");

        if (!string.IsNullOrEmpty(coupon))
        {
            var c = await _context.Coupons.FirstOrDefaultAsync(cp => cp.Code == coupon && cp.IsActive);
            if (c != null && items.Sum(i => i.SubTotal) >= c.MinOrderAmount)
            {
                discount = Math.Round(items.Sum(i => i.SubTotal) * (c.DiscountPercent / 100.0m), 2);
            }
            else
            {
                HttpContext.Session.Remove("AppliedCoupon");
                coupon = null;
            }
        }

        var model = new CartViewModel
        {
            Items = items,
            DiscountAmount = discount,
            AppliedCouponCode = coupon
        };

        return View(model);
    }

    // POST: /Cart/AddToCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.StockQuantity <= 0)
        {
            TempData["ErrorMessage"] = "Item is currently out of stock.";
            return RedirectToAction("Index", "Shop");
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        string sessionCartId = GetOrCreateCartSessionId();

        var existingItem = await _context.CartItems.FirstOrDefaultAsync(c =>
            c.ProductId == productId && (c.UserId == userId || (userId == null && c.SessionCartId == sessionCartId)));

        if (existingItem != null)
        {
            existingItem.Quantity = Math.Min(product.StockQuantity, existingItem.Quantity + quantity);
        }
        else
        {
            var cartItem = new CartItem
            {
                ProductId = productId,
                UserId = userId,
                SessionCartId = sessionCartId,
                Quantity = Math.Min(product.StockQuantity, Math.Max(1, quantity)),
                UnitPrice = product.Price,
                AddedAt = DateTime.UtcNow
            };
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Added \"{product.Name}\" to your cart!";

        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/UpdateQuantity
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int change)
    {
        var item = await _context.CartItems.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartItemId);
        if (item != null)
        {
            int newQty = item.Quantity + change;
            if (newQty <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = Math.Min(item.Product.StockQuantity, newQty);
            }
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Item removed from cart.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/ApplyCoupon
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
        {
            TempData["ErrorMessage"] = "Please enter a valid promo code.";
            return RedirectToAction(nameof(Index));
        }

        var cleanCode = couponCode.Trim().ToUpper();
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == cleanCode && c.IsActive);
        var items = await GetCurrentCartItemsAsync();
        decimal subTotal = items.Sum(i => i.SubTotal);

        if (coupon == null)
        {
            TempData["ErrorMessage"] = $"Promo code '{cleanCode}' is invalid or expired.";
        }
        else if (subTotal < coupon.MinOrderAmount)
        {
            TempData["ErrorMessage"] = $"Coupon '{cleanCode}' requires a minimum subtotal of ₹{coupon.MinOrderAmount:0.00}.";
        }
        else
        {
            HttpContext.Session.SetString("AppliedCoupon", cleanCode);
            TempData["SuccessMessage"] = $"Promo '{cleanCode}' applied! You saved {coupon.DiscountPercent}%.";
        }

        return RedirectToAction(nameof(Index));
    }
}
