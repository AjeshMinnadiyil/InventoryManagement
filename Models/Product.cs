
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string SKU { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 10;

    // Foreign Keys
    public int CategoryId { get; set; }

    public int SupplierId { get; set; }

    // Navigation Properties
    public Category? Category { get; set; }

    public Supplier? Supplier { get; set; }
}
