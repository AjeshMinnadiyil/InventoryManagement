
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class StockController : Controller
{
    private readonly ApplicationDbContext _context;

    public StockController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var transactions = await _context.StockTransactions
            .Include(t => t.Product)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return View(transactions);
    }

    public async Task<IActionResult> Create()
    {
        await LoadProductsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        StockTransaction transaction)
    {
        if (!Enum.IsDefined(
            typeof(StockTransactionType),
            transaction.TransactionType))
        {
            ModelState.AddModelError(
                nameof(transaction.TransactionType),
                "Select a valid transaction type.");
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.ProductId == transaction.ProductId);

        if (product == null)
        {
            ModelState.AddModelError(
                nameof(transaction.ProductId),
                "Please select a valid product.");
        }
        else if (transaction.Quantity > 0)
        {
            if (transaction.TransactionType ==
                    StockTransactionType.StockOut &&
                transaction.Quantity > product.Quantity)
            {
                ModelState.AddModelError(
                    nameof(transaction.Quantity),
                    "Insufficient stock available.");
            }
        }

        if (!ModelState.IsValid)
        {
            await LoadProductsAsync(transaction.ProductId);
            return View(transaction);
        }

        await using var dbTransaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            if (transaction.TransactionType ==
                StockTransactionType.StockIn)
            {
                product!.Quantity = checked(
                    product.Quantity + transaction.Quantity);
            }
            else
            {
                product!.Quantity -= transaction.Quantity;
            }

            transaction.TransactionDate = DateTime.UtcNow;

            _context.StockTransactions.Add(transaction);

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            TempData["Success"] =
                "Stock transaction saved successfully!";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            await dbTransaction.RollbackAsync();

            ModelState.AddModelError(
                string.Empty,
                "Unable to save the stock transaction. Please try again.");

            await LoadProductsAsync(transaction.ProductId);
            return View(transaction);
        }
    }

    private async Task LoadProductsAsync(int? selectedProductId = null)
    {
        ViewBag.Products = new SelectList(
            await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync(),
            "ProductId",
            "Name",
            selectedProductId);
    }
}
