using System.ComponentModel.DataAnnotations;

namespace Warehouse;

public class ProductWarehouseRequest
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }

}