using System;
using ScanBarcode.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
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
        [Route("CheckDONumber")]
        public IActionResult CheckDONumber(string doNumber)
        {
            bool exists = _context.DeliveryOrders.Any(deliveryOrder => deliveryOrder.Donumber == doNumber);
            return Json(new { exists });
        }

        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        [Route("Detail")]
        public IActionResult Detail()
        {
            return View();
        }
    }
}
