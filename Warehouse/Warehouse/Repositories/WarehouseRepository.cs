using System.Data.SqlClient;

namespace Warehouse.Controllers;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CheckIfProductExistsAsync(SqlConnection con, SqlTransaction transaction, int idProduct)
    {
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct", con, transaction);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<bool> CheckIfWarehouseExistsAsync(SqlConnection con, SqlTransaction transaction, int idWarehouse)
    {
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse", con,
            transaction);
        cmd.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<int?> GetMatchingOrderAsync(SqlConnection con, SqlTransaction transaction, int idProduct,
        double amount, DateTime createdAt)
    {
        using var cmd = new SqlCommand(
            "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL",
            con, transaction);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (int?)result : null;
    }

    public async Task<bool> CheckIfOrderFulfilledAsync(SqlConnection con, SqlTransaction transaction, int idOrder)
    {
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder", con,
            transaction);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task UpdateOrderFulfilledAtAsync(SqlConnection con, SqlTransaction transaction, int idOrder)
    {
        using var cmd = new SqlCommand("UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder", con,
            transaction);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertProductWarehouseAsync(SqlConnection con, SqlTransaction transaction,
        ProductWarehouseRequest request, int idOrder)
    {
        using var cmd = new SqlCommand(
            "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
            "OUTPUT INSERTED.IdProductWarehouse " +
            "VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE())",
            con, transaction);
        cmd.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        cmd.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        cmd.Parameters.AddWithValue("@Amount", request.Amount);
        cmd.Parameters.AddWithValue("@Price",
            (await GetProductPriceAsync(con, transaction, request.IdProduct)) * (decimal)request.Amount);
        var result = (int)await cmd.ExecuteScalarAsync();
        return result;
    }

    public async Task<decimal> GetProductPriceAsync(SqlConnection con, SqlTransaction transaction, int idProduct)
    {
        using var cmd = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct", con, transaction);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        var price = (decimal)await cmd.ExecuteScalarAsync();
        return price;
    }
}