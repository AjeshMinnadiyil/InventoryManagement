
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}
