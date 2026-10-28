using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkEasePro.Data;
using ParkEasePro.Models;

namespace ParkEasePro.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Orders
    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    // GET: /Orders/Details/5
    public async Task<IActionResult> Details(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null || (order.UserId != userId && !isAdmin))
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: /Orders/Invoice/5
    public async Task<IActionResult> Invoice(int id)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        bool isAdmin = User.IsInRole("Admin");

        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null || (order.UserId != userId && !isAdmin))
        {
            return NotFound();
        }

        return View(order);
    }
}
