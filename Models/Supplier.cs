
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
