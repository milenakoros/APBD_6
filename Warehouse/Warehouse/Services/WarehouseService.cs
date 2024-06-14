using System.Data.SqlClient;
using Warehouse.Controllers;

namespace Warehouse.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _repository;
    private readonly IConfiguration _configuration;

    public WarehouseService(IWarehouseRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<int> AddProductToWarehouseAsync(ProductWarehouseRequest request)
    {
        using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await con.OpenAsync();
        using var transaction = con.BeginTransaction();
        try
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Amount should be greater than 0");
            }


            // Sprawdzamy, czy produkt istnieje
            var productExists = await _repository.CheckIfProductExistsAsync(con, transaction, request.IdProduct);
            if (!productExists)
            {
                throw new Exception("Product not found");
            }

            // Sprawdzamy, czy magazyn istnieje
            var warehouseExists = await _repository.CheckIfWarehouseExistsAsync(con, transaction,request.IdWarehouse);
            if (!warehouseExists)
            {
                throw new Exception("Warehouse not found");
            }

            // Sprawdzamy, czy istnieje odpowiednie zamówienie
            var orderId = await _repository.GetMatchingOrderAsync(con, transaction,request.IdProduct, request.Amount, request.CreatedAt);
            if (orderId == null)
            {
                throw new Exception("Matching order not found");
            }

            // Sprawdzamy, czy zamówienie zostało zrealizowane
            var isOrderFulfilled = await _repository.CheckIfOrderFulfilledAsync(con, transaction,orderId.Value);
            if (isOrderFulfilled)
            {
                throw new Exception("Order already fulfilled");
            }

            // Aktualizujemy kolumnę FullfilledAt
            await _repository.UpdateOrderFulfilledAtAsync(con, transaction, orderId.Value);

            // Wstawiamy rekord do Product_Warehouse
            var productWarehouseId =
                await _repository.InsertProductWarehouseAsync(con, transaction, request, orderId.Value);

            transaction.Commit();

            return productWarehouseId;

        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }

    }
}