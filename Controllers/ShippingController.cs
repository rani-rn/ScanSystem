using System;
using ScanBarcode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ScanBarcode.Controllers
{

    public class ShippingController : Controller
    {
        private readonly ScanSystemContext _context;

        public ShippingController(ScanSystemContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("Shipping")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ListModel = _context.ProdModels.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeliveryOrder deliveryOrder)
        {
            if (ModelState.IsValid)
            {
                _context.DeliveryOrders.Add(deliveryOrder);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ListModel = _context.ProdModels.ToList();
            return View(deliveryOrder);
        }
public IActionResult CheckDONumber(string doNumber)
{
    bool exists = _context.DeliveryOrders.Any(deliveryOrder => deliveryOrder.Donumber == doNumber);
    return Json(new { exists });
}




        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }


    }

}