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
        if (deliveryOrder.TotalItems >= deliveryOrder.Qty)
        {
            statusDO = "completed";
        }
        else
        {
            statusDO = "not completed";
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

        var items = await _context.MasterItems
            .Where(mi => mi.SerialNumber == input || mi.RfidtagId == rfidtagId)
            .Select(mi => new
            {
                mi.SerialNumber,
                mi.Model,
                Rfidtag = _context.Rfidtags
                    .Where(r => r.Id == mi.RfidtagId)
                    .Select(r => r.TagNumber)
                    .FirstOrDefault(),
            })
            .ToListAsync();

        if (!items.Any())
        {
            return NotFound(new { error = "Serial Number atau RFID tidak ditemukan!" });
        }

        var totalRfidItems = rfidtagId != 0
            ? await _context.MasterItems.CountAsync(m => m.RfidtagId == rfidtagId)
            : 0;

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

        var totalItems = await _context.MasterTables
            .CountAsync(mt => mt.Donumber == deliveryOrder.Donumber);

        if (totalItems >= deliveryOrder.Qty)
        {
            return BadRequest(new { error = "Jumlah item sudah memenuhi permintaan!" });
        }

        var result = items.Select(item => new
        {
            item.SerialNumber,
            item.Model,
            item.Rfidtag,
            TotalRfidItems = totalRfidItems
        });

        return Ok(result);
    }

    [HttpPost("move-to-mastertable/{doId}")]
    public async Task<IActionResult> MoveToMasterTable(int doId, [FromBody] List<string> serialNumbers)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
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
            var currentTotalItems = await _context.MasterTables
                                        .CountAsync(mt => mt.Donumber == deliveryOrder.Donumber);
            var demandQty = deliveryOrder.Qty;

            if (currentTotalItems + actualQty > demandQty)
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
                    Qty = actualQty
                };
                _context.Shipments.Add(shipment);
            }
            else
            {
                shipment.Qty += actualQty;
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

            await transaction.CommitAsync();

            return Ok(new { message = "Scanned items successfully moved to MasterTable", actualQty = shipment.Qty });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
        }
    }

}