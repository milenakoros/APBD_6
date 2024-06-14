using System.Data.SqlClient;

namespace Warehouse.Controllers;

public class WarehouseRepository : IWarehouseRepository
{
    public IConfiguration _configuration { get; }

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> CheckIfProductExistsAsync(int idProduct)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        await using var cmd = new SqlCommand("SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct", con);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        int count = (int) await cmd.ExecuteScalarAsync();
        return count > 0;

    }

    public async Task<bool> CheckIfWarehouseExistsAsync(int idWarehouse)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse", con);
        cmd.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task<int?> GetMatchingOrderAsync(int idProduct, double amount, DateTime createdAt)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        using var cmd = new SqlCommand(
            "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt AND FulfilledAt IS NULL",
            con);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (int?)result : null;
    }

    public async Task<bool> CheckIfOrderFulfilledAsync(int idOrder)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        using var cmd = new SqlCommand("SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder", con);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        var count = (int)await cmd.ExecuteScalarAsync();
        return count > 0;
    }

    public async Task UpdateOrderFulfilledAtAsync(SqlConnection con, SqlTransaction transaction, int idOrder)
    {
        using var cmd = new SqlCommand("UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder", con, transaction);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertProductWarehouseAsync(SqlConnection con, SqlTransaction transaction, ProductWarehouseRequest request, int idOrder)
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
        cmd.Parameters.AddWithValue("@Price", await GetProductPriceAsync(request.IdProduct) * request.Amount);
        var result = (int)await cmd.ExecuteScalarAsync();
        return result;
    }

    public async Task<double> GetProductPriceAsync(int idProduct)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();

        using var cmd = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @IdProduct", con);
        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        var price = (double)await cmd.ExecuteScalarAsync();
        return price;
    }
}