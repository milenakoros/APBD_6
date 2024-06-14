namespace Warehouse.Controllers;

public interface IWarehouseRepository
{
    Task<bool> CheckIfProductExistsAsync(int idProduct);
    Task<bool> CheckIfWarehouseExistsAsync(int idWarehouse);
    Task<int?> GetMatchingOrderAsync(int idProduct, int amount, DateTime createdAt);
    Task<bool> CheckIfOrderFulfilledAsync(int idOrder);
    Task UpdateOrderFulfilledAtAsync(int idOrder);
    Task<int> InsertProductWarehouseAsync(ProductWarehouseRequest request, int idOrder);
    Task<double> GetProductPriceAsync(int idProduct);
}