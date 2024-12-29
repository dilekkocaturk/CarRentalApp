using carRentalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;

namespace carRentalApp.Controllers
{
    [Authorize(Roles = "admin")] // Restrict access to admin only
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin options page
        [HttpGet]
        public IActionResult AdminOptions()
        {
            return View();
        }

        // List all cars in the database
        [HttpGet]
        public IActionResult List()
        {
            var cars = _context.Cars.ToList();
            return View(cars);
        }

        // Display registration page for adding a new car
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle car registration and save to the database
        [HttpPost]
        public IActionResult Register(Car car)
        {
            // Check if the plate number already exists
            if (_context.Cars.Any(c => c.PlateNumber == car.PlateNumber))
            {
                ModelState.AddModelError("PlateNumber", "This plate number already exists. Please use a different one.");
                return View(car);
            }
            _context.Cars.Add(car);
            _context.SaveChanges();
            return RedirectToAction("AdminInfo");
        }

        [HttpGet]
        public IActionResult AdminInfo()
        {
            return View();
        }

        // Method to return chart data for cars (Active and Idle percentages)
        [HttpGet]
        public IActionResult GetChartData()
        {
            var cars = _context.Cars.ToList();

            // Transform the data into a format suitable for graphing
            var chartData = cars.Select(car => new
            {
                CarName = car.CarName,
                ActivePercentage = (car.ActiveWorkingTime / (7 * 24)) * 100, // Active working time percentage
                IdlePercentage = (car.IdleTime / (7 * 24)) * 100 // Idle time percentage
            }).ToList();

            return Json(chartData);
        }
    }
}