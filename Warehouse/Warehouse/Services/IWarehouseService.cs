namespace Warehouse.Services;

public interface IWarehouseService
{
    Task<int> AddProductToWarehouseAsync(ProductWarehouseRequest request);
}