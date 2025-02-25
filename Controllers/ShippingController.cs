using System;
using ScanBarcode.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace ScanBarcode.Controllers
{
    [Route("Shipping")]
    [ApiController]
    public class ShippingController : Controller
    {
        private readonly ScanSystemContext _context;

        public ShippingController(ScanSystemContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var SOData = _context.SOLists
                        .Select(so => new
                        {
                            so.SOId,
                            so.SONumber,
                            so.Destination,
                            so.Date,
                            TotalDO = _context.DeliveryOrders.Count(d => d.SONumber == so.SONumber)
                        }).ToList();
            return View(SOData);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.ListModel = _context.ProdModels.ToList();
            return View();
        }
        [HttpPost]
        [Route("CreatePost")]
        public IActionResult CreatePost([FromBody] SOList request)
        {
            if (request == null || request.DeliveryOrders == null || request.DeliveryOrders.Count == 0)
            {
                Console.WriteLine(" Invalid request data received.");
                return BadRequest(new { message = "Invalid request data." });
            }

            bool isSONumberExists = _context.SOLists.Any(so => so.SONumber == request.SONumber);
            if (isSONumberExists)
            {
                return BadRequest(new { message = "SONumber already exists. Please use a different SONumber." });
            }

            Console.WriteLine($" SONumber: {request.SONumber}, Destination: {request.Destination}, Date: {request.Date}");

            foreach (var order in request.DeliveryOrders)
            {
                Console.WriteLine($" DO: {order.Donumber}, ModelId: {order.ModelId}, Qty: {order.Qty}");
            }

            try
            {
                var soList = new SOList
                {
                    SONumber = request.SONumber,
                    Destination = request.Destination,
                    Date = request.Date
                };

                _context.SOLists.Add(soList);
                _context.SaveChanges();
                var newSOID = soList.SOId;

                var deliveryOrders = request.DeliveryOrders.Select(order => new DeliveryOrder
                {
                    SONumber = request.SONumber,
                    Donumber = order.Donumber,
                    ModelId = order.ModelId,
                    Qty = order.Qty,
                    RequestedDate = request.Date,
                    Destination = request.Destination
                }).ToList();

                _context.DeliveryOrders.AddRange(deliveryOrders);
                _context.SaveChanges();

                return Ok(new { message = "Data saved successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
                return BadRequest(new { message = "Error: " + ex.Message });
            }
        }
        [HttpGet]
        [Route("CheckSONumber")]
        public IActionResult CheckSONumber(string soNumber)
        {
            bool exists = _context.SOLists.Any(so => so.SONumber == soNumber);
            return Json(new { exists });
        }

        [HttpGet]
        [Route("CheckDONumber")]
        public IActionResult CheckDONumber(string doNumber)
        {
            bool exists = _context.DeliveryOrders.Any(deliveryOrder => deliveryOrder.Donumber == doNumber);
            return Json(new { exists });
        }
        
        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var soList = _context.SOLists.FirstOrDefault(so => so.SOId == id);

            if (soList == null)
            {
                return NotFound("SO not found.");
            }

            ViewBag.ListModel = _context.ProdModels.ToList();

            return View(soList);
        }

        [HttpGet]
        [Route("GetSOId")]
        public IActionResult GetSOId(string soNumber)
        {
            var so = _context.SOLists
                .Where(s => s.SONumber == soNumber)
                .Select(s => new { s.SOId })
                .FirstOrDefault();

            if (so == null)
            {
                return NotFound(new { success = false, message = "SO not found." });
            }

            return Json(new { success = true, soId = so.SOId });
        }


        [HttpGet]
        [Route("GetShippingData/{soId}")]
        public IActionResult GetShippingData(int soId)
        {
            var soList = _context.SOLists.FirstOrDefault(so => so.SOId == soId);
            if (soList == null)
            {
                return NotFound(new { success = false, message = "SO not found." });
            }

            var deliveryOrders = _context.DeliveryOrders
                .Where(d => d.SONumber == soList.SONumber)
                .Select(d => new
                {
                    d.Donumber,
                    d.ModelId,
                    ModelName = _context.ProdModels
                                .Where(m => m.ModelId == d.ModelId)
                                .Select(m => m.ModelName)
                                .FirstOrDefault(),
                    d.Qty
                })
                .ToList();

            return Json(new { success = true, so = soList, data = deliveryOrders });
        }

        [HttpPost]
        [Route("UpdatePost/{id}")]
        public IActionResult UpdatePost(int id, [FromBody] SOList request)
        {
            if (request == null || request.DeliveryOrders == null || request.DeliveryOrders.Count == 0)
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            var soList = _context.SOLists.FirstOrDefault(so => so.SOId == id);
            if (soList == null)
            {
                return NotFound(new { message = "SO not found." });
            }
            bool isSONumberExists = _context.SOLists.Any(so => so.SONumber == request.SONumber && so.SOId != id);
            if (isSONumberExists)
            {
                return BadRequest(new { message = "SONumber already exists. Please use a different SONumber." });
            }
            var existingDONumbers = _context.DeliveryOrders.Select(d => d.Donumber).ToList();
            foreach (var order in request.DeliveryOrders)
            {
                if (existingDONumbers.Contains(order.Donumber))
                {
                    return BadRequest(new { message = $"DONumber {order.Donumber} already exists." });
                }
            }

            soList.SONumber = request.SONumber;
            soList.Destination = request.Destination;
            soList.Date = request.Date;

            var existingDOs = _context.DeliveryOrders.Where(d => d.SONumber == soList.SONumber).ToList();
            _context.DeliveryOrders.RemoveRange(existingDOs);
            _context.DeliveryOrders.AddRange(request.DeliveryOrders.Select(order => new DeliveryOrder
            {
                SONumber = soList.SONumber,
                Donumber = order.Donumber,
                ModelId = order.ModelId,
                Qty = order.Qty,
                RequestedDate = request.Date,
                Destination = request.Destination
            }));

            _context.SaveChanges();
            return Ok(new { message = "Data updated successfully!" });
        }


    }
}
