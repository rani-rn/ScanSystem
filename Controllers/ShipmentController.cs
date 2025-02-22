using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanBarcode.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/shipments")]
[ApiController]
public class ShipmentsController : ControllerBase
{
    private readonly ScanSystemContext _context;

    public ShipmentsController(ScanSystemContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult GetShipments()
    {
        var shipments = _context.Shipments
            .Join(
                _context.ProdModels, 
                s => s.ModelId,
                m => m.ModelId,
                (s, m) => new  
                {
                    s.ShipmentId,
                    s.Doid,
                    s.ShipmentDate,
                    s.Destination,
                    s.ModelId,
                    ModelName = m.ModelName, 
                    s.Qty,
                
                })
            .ToList();

        return Ok(shipments);
    }
}
