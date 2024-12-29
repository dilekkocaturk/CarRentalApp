using carRentalApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.Eventing.Reader;

namespace carRentalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Login page
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Handle user login
        [HttpPost]
        public async Task<IActionResult> Index(User user, string returnUrl = null)
        {
            // Search for the user in the database
            // Case-sensitive username comparison
            var existingUser = _dbContext.Users
                .Where(u => u.UserName == user.UserName)
                .AsEnumerable()
                .FirstOrDefault(u => u.UserName.Equals(user.UserName, StringComparison.Ordinal));

            if (existingUser == null)
            {
                TempData["LoginError"] = "Invalid username or password.";
                return RedirectToAction("Index");
            }

            // Check the password
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash)));
                if (hashedPassword != existingUser.PasswordHash)
                {
                    TempData["LoginError"] = "Invalid username or password.";
                    return RedirectToAction("Index");
                }
            }

            // Log the user in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.UserName),
                new Claim(ClaimTypes.Role, existingUser.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Redirect to the appropriate page based on the user's role
            return RedirectToAction(existingUser.Role == "admin" ? "AdminOptions" : "Update", existingUser.Role == "admin" ? "Admin" : "User");
        }

        // Logout user
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // SignUp page
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // Handle sign up for a new user
        [HttpPost]
        public IActionResult SignUp(User user)
        {
            // Check if the username already exists (case-sensitive)
            if (_dbContext.Users
                .AsEnumerable()
                .Any(u => u.UserName.Equals(user.UserName, StringComparison.Ordinal)))
            {
                ModelState.AddModelError("UserName", "This username is already taken. Please choose a different one.");
                return View(user);
            }

            // Hash the password
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash)));
                user.PasswordHash = hashedPassword;
            }

            // Set the user's role to "user"
            user.Role = "user";

            // Add the user to the database
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Use TempData to show a message on the login page
            TempData["Message"] = "Account successfully created! Please log in.";
            return RedirectToAction("Index");
        }
    }
}