using Microsoft.AspNetCore.Http.Features;
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
                Destination = d.Destination,
                Qty = d.Qty,
                ContNo = d.ContNo,
                ModelName = _context.ProdModels
                    .Where(m => m.ModelId == d.ModelId)
                    .Select(m => m.ModelName)
                    .FirstOrDefault(),
                TotalItems = _context.MasterTables
                            .Where(mt => mt.Donumber == d.Donumber)
                            .Count()
            })
            .FirstOrDefaultAsync();

        if (deliveryOrder == null)
        {
            return NotFound(new { message = "Delivery Order not found" });
        }

        string statusDO;
        if (deliveryOrder.TotalItems >= deliveryOrder.Qty){
            statusDO = "Completed";
        } else {
            statusDO = "Not Completed";
        }
        var result = new
    {
        deliveryOrder.Donumber,
        deliveryOrder.Destination,
        deliveryOrder.Qty,
        deliveryOrder.ContNo,
        deliveryOrder.ModelName,
        deliveryOrder.TotalItems,
        Status = statusDO
    };
        return Ok(result);
    }

    [HttpGet("validate/{input}")]
    public async Task<IActionResult> ValidateNumber(string input)
    {
        var rfidtagId = await _context.Rfidtags
                        .Where(r => r.TagNumber == input)
                        .Select(r => r.Id)
                        .FirstOrDefaultAsync();

        var itemQuery = _context.MasterItems
            .Where(mi => mi.SerialNumber == input ||
                        mi.RfidtagId == rfidtagId)
            .Select(mi => new
            {
                mi.SerialNumber,
                mi.Model,
                Rfidtag = _context.Rfidtags
                .Where(r => r.Id == mi.RfidtagId)
                .Select(r => r.TagNumber).FirstOrDefault()
            });


        var items = await itemQuery.ToListAsync();

        if (!items.Any())
        {
            return NotFound(new { error = "Serial Number atau RFID tidak ditemukan!" });
        }
        var deliveryOrder = await _context.DeliveryOrders
            .Where(d => _context.ProdModels
                .Where(m => m.ModelId == d.ModelId)
                .Select(m => m.ModelName)
                .Contains(items.First().Model))
            .FirstOrDefaultAsync();

        if (deliveryOrder == null)
        {
            return BadRequest(new { error = "Model tidak sesuai dengan Delivery Order!" });
        }
        return Ok(items);
    }

    [HttpPost("remove-rfid")]
    public async Task<IActionResult> RemoveRfid([FromBody] string serialNumber)
    {
        var item = await _context.MasterItems.FirstOrDefaultAsync(mi => mi.SerialNumber == serialNumber);
        if (item == null)
        {
            return NotFound(new { error = "Serial Number Not Found!" });
        }

        item.RfidtagId = null;
        await _context.SaveChangesAsync();

        return Ok(new { message = "RFID success deleted" });
    }

    [HttpPost("move-to-mastertable/{doId}")]
    public async Task<IActionResult> MoveToMasterTable(int doId, [FromBody] List<string> serialNumbers)
    {
        var deliveryOrder = await _context.DeliveryOrders.FirstOrDefaultAsync(d => d.Doid == doId);
        if (deliveryOrder == null)
        {
            return NotFound(new { error = "Delivery Order Not Found!" });
        }

        var modelName = await _context.ProdModels
            .Where(m => m.ModelId == deliveryOrder.ModelId)
            .Select(m => m.ModelName)
            .FirstOrDefaultAsync();
        if (string.IsNullOrEmpty(modelName))
        {
            return NotFound(new { error = "Model Name Not Found!" });
        }

        var masterItems = await _context.MasterItems
            .Where(mi => serialNumbers.Contains(mi.SerialNumber))
            .ToListAsync();
        if (!masterItems.Any())
        {
            return NotFound(new { error = "No matching scanned items found!" });
        }

        var actualQty = masterItems.Count;
        var demandQty = deliveryOrder.Qty;

        if (actualQty + deliveryOrder.Qty > demandQty)
        {
            return BadRequest(new { error = "Demand quantity exceeded!" });
        }

        foreach (var item in masterItems)
        {
            var mastertableEntry = new MasterTable
            {
                SerialNumber = item.SerialNumber,
                Model = modelName,
                Donumber = deliveryOrder.Donumber,
                LineProduction = item.LineProduction,
                Destination = deliveryOrder.Destination,
                ContNo = deliveryOrder.ContNo,
                ShipmentId = null,
                ShipmentDateTime = DateTime.Now
            };
            _context.MasterTables.Add(mastertableEntry);
        }
        await _context.SaveChangesAsync();

        _context.MasterItems.RemoveRange(masterItems);
        await _context.SaveChangesAsync();

        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(s => s.Doid == deliveryOrder.Doid);
        if (shipment == null)
        {
            shipment = new Shipment
            {
                Doid = deliveryOrder.Doid,
                ShipmentDate = DateTime.Now,
                ContNo = deliveryOrder.ContNo,
                Destination = deliveryOrder.Destination,
                ModelId = deliveryOrder.ModelId,
                Qty = masterItems.Count
            };
            _context.Shipments.Add(shipment);
        }
        else
        {
            shipment.Qty += masterItems.Count;
        }
        await _context.SaveChangesAsync();

        var masterTablesUpdate = await _context.MasterTables
            .Where(mt => mt.Donumber == deliveryOrder.Donumber)
            .ToListAsync();
        foreach (var mt in masterTablesUpdate)
        {
            mt.ShipmentId = shipment.ShipmentId;
        }
        await _context.SaveChangesAsync();

        return Ok(new { message = "Scanned items successfully moved to MasterTable", actualQty = shipment.Qty });
    }

}