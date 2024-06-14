using System.Data.SqlClient;

namespace Warehouse.Controllers;

public interface IWarehouseRepository
{
    Task<bool> CheckIfProductExistsAsync(SqlConnection con, SqlTransaction transaction, int idProduct);
    Task<bool> CheckIfWarehouseExistsAsync(SqlConnection con, SqlTransaction transaction, int idWarehouse);
    Task<int?> GetMatchingOrderAsync(SqlConnection con, SqlTransaction transaction, int idProduct, double amount, DateTime createdAt);
    Task<bool> CheckIfOrderFulfilledAsync(SqlConnection con, SqlTransaction transaction, int idOrder);
    Task UpdateOrderFulfilledAtAsync(SqlConnection con, SqlTransaction transaction, int idOrder);
    Task<int> InsertProductWarehouseAsync(SqlConnection con, SqlTransaction transaction, ProductWarehouseRequest request, int idOrder);
    Task<decimal> GetProductPriceAsync(SqlConnection con, SqlTransaction transaction, int idProduct);
}