using System.Data.SqlClient;

namespace Warehouse.Controllers;

public interface IWarehouseRepository
{
    Task<bool> CheckIfProductExistsAsync(int idProduct);
    Task<bool> CheckIfWarehouseExistsAsync(int idWarehouse);
    Task<int?> GetMatchingOrderAsync(int idProduct, double amount, DateTime createdAt);
    Task<bool> CheckIfOrderFulfilledAsync(int idOrder);
    Task UpdateOrderFulfilledAtAsync(SqlConnection con, SqlTransaction transaction, int idOrder);
    Task<int> InsertProductWarehouseAsync(SqlConnection con, SqlTransaction transaction, ProductWarehouseRequest request, int idOrder);
    Task<Decimal>  GetProductPriceAsync(int idProduct);
}