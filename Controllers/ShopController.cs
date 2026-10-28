using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;
using ParkEasePro.Models.ViewModels;

namespace ParkEasePro.Controllers;

public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Shop
    public async Task<IActionResult> Index(string? q, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sort, int page = 1)
    {
        int pageSize = 9;
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Brand.ToLower().Contains(term) || p.ShortDescription.ToLower().Contains(term));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Sorting
        query = sort switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            _ => query.OrderByDescending(p => p.CreatedAt) // default newest
        };

        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages < 1) totalPages = 1;
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var categories = await _context.Categories.Where(c => c.IsActive).Include(c => c.Products).ToListAsync();

        var model = new ShopCatalogViewModel
        {
            Products = products,
            Categories = categories,
            SelectedCategoryId = categoryId,
            SearchTerm = q,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sort,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalCount
        };

        return View(model);
    }

    // GET: /Shop/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            TempData["ErrorMessage"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.RelatedProducts = await _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Take(4)
            .ToListAsync();

        return View(product);
    }

    // POST: /Shop/AddReview
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int productId, int rating, string comment)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var product = await _context.Products.Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return NotFound();

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = Math.Clamp(rating, 1, 5),
            Comment = comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Update product average rating
        var allReviews = await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync();
        product.ReviewCount = allReviews.Count;
        product.Rating = Math.Round(allReviews.Average(r => r.Rating), 1);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Thank you for sharing your verified review!";
        return RedirectToAction(nameof(Details), new { id = productId });
    }
}
