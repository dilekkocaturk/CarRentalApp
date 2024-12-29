using carRentalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;

namespace carRentalApp.Controllers
{
    [Authorize(Roles = "user")]  // Restrict access to user only
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject the ApplicationDbContext
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display the car update page when the user is logged in
        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        // Handle car update for the user
        [HttpPost]
        public IActionResult Update(Car car)
        {
            // Search for the car in the database by its plate number
            var existingCar = _context.Cars.FirstOrDefault(c => c.PlateNumber == car.PlateNumber);

            // Update the car's active working time and maintenance time
            existingCar.ActiveWorkingTime = car.ActiveWorkingTime;
            existingCar.MaintenanceTime = car.MaintenanceTime;

            // Calculate the idle time (total time - active working time - maintenance time)
            existingCar.IdleTime = (7 * 24) - (existingCar.ActiveWorkingTime + existingCar.MaintenanceTime);

            // Update the database with the new car data
            _context.SaveChanges();

            // Redirect to the user info page after successful update
            return RedirectToAction("UserInfo");
        }

        // Method to retrieve the car name based on its plate number
        [HttpGet]
        public IActionResult GetCarName(string plateNumber)
        {
            // Search for the car in the database using the plate number
            var car = _context.Cars.FirstOrDefault(c => c.PlateNumber == plateNumber);
            if (car != null)
            {
                // If the car is found, return the car name
                return Json(new
                {
                    carName = car.CarName
                });
            }

            // If the car is not found, return an error message
            return NotFound(new { message = "Car not found." });
        }

        // Display the user info page after successfully saving car data
        [HttpGet]
        public IActionResult UserInfo()
        {
            return View();
        }
    }
}