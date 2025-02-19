using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanBarcode.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/deliveryorder")]
[ApiController]
public class DOController : ControllerBase
{
    private readonly ScanSystemContext _context;

    public DOController(ScanSystemContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveryOrders()
    {
        var deliveryOrders = await _context.DeliveryOrders
            .Select(d => new { d.Doid, d.Donumber })
            .ToListAsync();

        if (!deliveryOrders.Any())
        {
            return NotFound(new { message = "No Delivery Orders found" });
        }

        return Ok(deliveryOrders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDO(int id)
    {
        var deliveryOrder = await _context.DeliveryOrders
            .Where(d => d.Doid == id)
            .Select(d => new
            {
                d.Donumber,
                Destination = d.Destination ?? "Unknown Destination",
                Qty = d.Qty ?? 0,
                ModelName = _context.ProdModels
                    .Where(m => m.ModelId == d.ModelId)
                    .Select(m => m.ModelName)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (deliveryOrder == null)
        {
            return NotFound(new { message = "Delivery Order not found" });
        }

        return Ok(deliveryOrder);
    }

    [HttpGet("validate/{input}")]
public async Task<IActionResult> ValidateNumber(string input)
{
    var items = await _context.MasterItems
        .GroupJoin(
            _context.Rfidtags,
            mi => mi.RfidtagId,
            r => r.Id,
            (mi, rfidGroup) => new
            {
                mi.SerialNumber,
                RFIDs = rfidGroup.Select(r => r.TagNumber).ToList()
            }
        )
        .SelectMany(
            x => x.RFIDs.DefaultIfEmpty(), 
            (x, rfid) => new
            {
                x.SerialNumber,
                RFID = rfid ?? "-"
            }
        )
        .Where(item => item.SerialNumber == input || item.RFID == input)
        .ToListAsync();

    if (!items.Any())
    {
        return NotFound(new { error = "Serial Number atau RFID tidak ditemukan!" });
    }

    return Ok(items);
}

}