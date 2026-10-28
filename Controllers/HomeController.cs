using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;
using ParkEasePro.Models.ViewModels;

namespace ParkEasePro.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .ToListAsync();

        var featured = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsFeatured && p.StockQuantity > 0)
            .Take(8)
            .ToListAsync();

        var trending = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsTrending && p.StockQuantity > 0)
            .Take(8)
            .ToListAsync();

        var onSale = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsOnSale && p.StockQuantity > 0)
            .Take(8)
            .ToListAsync();

        var model = new HomeViewModel
        {
            Categories = categories,
            FeaturedProducts = featured,
            TrendingProducts = trending,
            OnSaleProducts = onSale
        };

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
