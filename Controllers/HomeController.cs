
using InventoryManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _context.Products.CountAsync();

            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            ViewBag.TotalSuppliers = await _context.Suppliers.CountAsync();

            ViewBag.LowStockProducts = await _context.Products
                .CountAsync(p => p.Quantity <= p.ReorderLevel);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
