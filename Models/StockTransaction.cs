
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models;

public enum StockTransactionType
{
    StockIn = 1,
    StockOut = 2
}

public class StockTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    [Required]
    public StockTransactionType TransactionType { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime TransactionDate { get; set; }
        = DateTime.UtcNow;

    [StringLength(250)]
    public string? Notes { get; set; }

    public Product? Product { get; set; }
}
