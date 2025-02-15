using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanBarcode.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private readonly ScanSystemContext _context;

    public ShipmentController(ScanSystemContext context)
    {
        _context = context;
    }

    [HttpGet("GetDeliveryOrders")]
    public async Task<IActionResult> GetDeliveryOrders()
    {
        var deliveryOrders = await _context.DeliveryOrders
            .Select(d => new { d.Doid, d.Donumber })
            .ToListAsync();

        return Ok(deliveryOrders);
    }

    [HttpGet("GetShipmentDetailsBySerial")]
    public async Task<IActionResult> GetShipmentDetailsBySerial(string serial)
    {
        var shipment = await _context.Shipments
            .Where(s => s.RfidtagId.ToString() == serial)
            .Select(s => new
            {
                s.ModelId,
                s.Destination,
                s.ShipmentDate,
                s.ContNo,
                s.Qty,
                Actual = _context.Shipments.Count(sh => sh.RfidtagId == s.RfidtagId) // Hitung aktual
            })
            .FirstOrDefaultAsync();

        if (shipment == null)
            return NotFound(new { success = false, message = "Serial Number not found!" });

        return Ok(new { success = true, data = shipment });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateShipment([FromBody] Shipment shipment)
    {
        if (shipment == null || shipment.RfidtagId == 0)
            return BadRequest(new { success = false, message = "Invalid serial number!" });

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Shipment confirmed!" });
    }
}
