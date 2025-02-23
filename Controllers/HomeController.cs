
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScanBarcode.Models;

namespace ScanBarcode.Controllers
{
    public class HomeController : Controller
    {
        private readonly ScanSystemContext _context;

        public HomeController(ScanSystemContext context)
        {
            _context = context;
        }

        //To Check Db Connection : http://localhost:5177/Home/TestConnection
        public IActionResult TestConnection()
        {
            try
            {
                _context.Database.CanConnect();
                return Content("Database connect!");
            }
            catch (Exception ex)
            {
                return Content("Error koneksi: " + ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preparation()
        {
            return View();
        }
        public IActionResult Package()
        {
            return View();
        }

        public IActionResult Shipping()
        {
            return RedirectToAction("Index", "Shipping");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
