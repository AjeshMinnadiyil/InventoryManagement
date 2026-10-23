
using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Product List
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Index(
        string? searchString,
        int? categoryId,
        int page = 1)
    {
        int pageSize = 5;

        if (page < 1)
            page = 1;

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsQueryable();

        // Search by product name or SKU
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(p =>
                p.Name.Contains(searchString) ||
                p.SKU.Contains(searchString));
        }

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        int totalProducts = await query.CountAsync();

        int totalPages = (int)Math.Ceiling(
            totalProducts / (double)pageSize);

        var products = await query
            .OrderBy(p => p.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.SearchString = searchString;
        ViewBag.CategoryId = categoryId;

        ViewBag.Categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(products);
    }


    // 2. Create - GET
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View();
    }

    // Create - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product)
    {
        if (await _context.Products
            .AnyAsync(p => p.SKU == product.SKU))
        {
            ModelState.AddModelError(
                nameof(product.SKU),
                "This SKU already exists.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(product.CategoryId,
                product.SupplierId);

            return View(product);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Product added successfully!";

        return RedirectToAction(nameof(Index));
    }

    // 3. Edit - GET
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        await LoadDropdownsAsync(
            product.CategoryId, product.SupplierId);

        return View(product);
    }

    // Edit - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(
        int id, Product product)
    {
        if (id != product.ProductId)
            return NotFound();

        if (await _context.Products.AnyAsync(
            p => p.SKU == product.SKU &&
                 p.ProductId != product.ProductId))
        {
            ModelState.AddModelError(
                nameof(product.SKU),
                "This SKU already exists.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(
                product.CategoryId, product.SupplierId);

            return View(product);
        }

        var existingProduct = await _context.Products
            .FindAsync(id);

        if (existingProduct == null)
            return NotFound();

        existingProduct.Name = product.Name;
        existingProduct.SKU = product.SKU;
        existingProduct.Price = product.Price;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.SupplierId = product.SupplierId;
        existingProduct.ReorderLevel = product.ReorderLevel;

        // Quantity will be managed through Stock In/Out.
        await _context.SaveChangesAsync();

        TempData["Success"] = "Product updated successfully!";

        return RedirectToAction(nameof(Index));
    }

    // 4. Product Details
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // 5. Delete - GET
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // Delete - POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        bool hasTransactions = await _context.StockTransactions
            .AnyAsync(t => t.ProductId == id);

        if (hasTransactions)
        {
            TempData["Error"] =
                "Cannot delete this product because stock transactions exist.";

            return RedirectToAction(nameof(Index));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Product deleted successfully!";

        return RedirectToAction(nameof(Index));
    }

    // Category and Supplier dropdowns
    private async Task LoadDropdownsAsync(
        int? categoryId = null, int? supplierId = null)
    {
        ViewBag.Categories = new SelectList(
            await _context.Categories.OrderBy(c => c.Name)
                .ToListAsync(),
            "CategoryId", "Name", categoryId);

        ViewBag.Suppliers = new SelectList(
            await _context.Suppliers.OrderBy(s => s.Name)
                .ToListAsync(),
            "SupplierId", "Name", supplierId);
    }
}
