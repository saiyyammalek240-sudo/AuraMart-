using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;
using ParkEasePro.Models.ViewModels;

namespace ParkEasePro.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var totalRev = await _context.Orders.Where(o => o.PaymentStatus == PaymentStatus.Paid).SumAsync(o => o.GrandTotal);
        var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var monthRev = await _context.Orders.Where(o => o.PaymentStatus == PaymentStatus.Paid && o.OrderDate >= thisMonth).SumAsync(o => o.GrandTotal);
        var ordersCount = await _context.Orders.CountAsync();
        var productsCount = await _context.Products.CountAsync();
        var lowStock = await _context.Products.Where(p => p.StockQuantity <= 10).ToListAsync();
        var activeCustomers = await _context.Orders.Select(o => o.UserId).Distinct().CountAsync();

        var recentOrders = await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .Take(8)
            .ToListAsync();

        // 7-day sales graph
        var dates = new List<string>();
        var dailyRevs = new List<decimal>();
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var nextDate = date.AddDays(1);
            var rev = await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid && o.OrderDate >= date && o.OrderDate < nextDate)
                .SumAsync(o => o.GrandTotal);

            dates.Add(date.ToString("ddd (dd/MM)"));
            dailyRevs.Add(rev);
        }

        // Category product counts
        var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
        var catLabels = categories.Select(c => c.Name).ToList();
        var catCounts = categories.Select(c => c.Products.Count).ToList();

        var model = new AdminEcommerceDashboardViewModel
        {
            TotalSalesRevenue = totalRev,
            MonthSalesRevenue = monthRev,
            TotalOrdersCount = ordersCount,
            TotalProductsCount = productsCount,
            LowStockProductsCount = lowStock.Count,
            ActiveCustomersCount = activeCustomers,
            RecentOrders = recentOrders,
            LowStockProducts = lowStock,
            RevenueDates = dates,
            DailyRevenues = dailyRevs,
            CategoryLabels = catLabels,
            CategoryCounts = catCounts
        };

        return View(model);
    }

    // GET: /Admin/Products
    public async Task<IActionResult> Products(int? categoryId, string? search)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.ToLower().Contains(search.Trim().ToLower()) || p.Brand.ToLower().Contains(search.Trim().ToLower()));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);
        var products = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return View(products);
    }

    // GET: /Admin/CreateProduct
    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: /Admin/CreateProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Slug))
        {
            product.Slug = product.Name.ToLower().Replace(" ", "-");
        }
        product.CreatedAt = DateTime.UtcNow;

        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Product \"{product.Name}\" created successfully.";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    // GET: /Admin/EditProduct/5
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    // POST: /Admin/EditProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Product \"{product.Name}\" updated successfully.";
            return RedirectToAction(nameof(Products));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "Id", "Name", product.CategoryId);
        return View(product);
    }

    // POST: /Admin/DeleteProduct/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product deleted.";
        }
        return RedirectToAction(nameof(Products));
    }

    // GET: /Admin/Orders
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    // POST: /Admin/UpdateOrderStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Order {order.OrderNumber} status updated to {status}.";
        }
        return RedirectToAction(nameof(Orders));
    }

    // GET: /Admin/ExportOrdersCsv
    public async Task<IActionResult> ExportOrdersCsv()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("OrderNumber,CustomerName,Email,OrderDate,ItemsCount,SubTotal,Discount,GrandTotal,Status,PaymentStatus,PaymentMethod,TrackingNumber");

        foreach (var o in orders)
        {
            sb.AppendLine($"\"{o.OrderNumber}\",\"{o.ShippingFullName}\",\"{o.User.Email}\",\"{o.OrderDate:yyyy-MM-dd HH:mm}\",{o.Items.Count},{o.SubTotal},{o.DiscountAmount},{o.GrandTotal},\"{o.Status}\",\"{o.PaymentStatus}\",\"{o.PaymentMethod}\",\"{o.TrackingNumber}\"");
        }

        byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"AuraMart_Orders_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
    }
}
