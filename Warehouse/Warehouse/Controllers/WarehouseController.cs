using Microsoft.AspNetCore.Mvc;
using Warehouse;
using Warehouse.Services;


[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehouseController(IWarehouseService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] ProductWarehouseRequest request)
    {
        try
        {
            var productWarehouseId = await _service.AddProductToWarehouseAsync(request);
            return Ok(new { IdProductWarehouse = productWarehouseId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
