using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace StudentRegistrationApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View();
            }

            var existing = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (existing != null)
            {
                ModelState.AddModelError("", "A student with this email already exists.");
                return View();
            }

            // Simple registration number generator
            int count = await _context.Students.CountAsync() + 1;
            string regNumber = $"STU-{1000 + count}";

            string passwordHash = HashPassword(password);

            var student = new Student
            {
                FullName = fullName,
                Email = email,
                RegistrationNumber = regNumber,
                PasswordHash = passwordHash
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Login after register
            HttpContext.Session.SetInt32("StudentId", student.Id);
            HttpContext.Session.SetString("StudentName", student.FullName);

            return RedirectToAction("Index", "Students");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            string passwordHash = HashPassword(password);

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email && s.PasswordHash == passwordHash);

            if (student == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            HttpContext.Session.SetInt32("StudentId", student.Id);
            HttpContext.Session.SetString("StudentName", student.FullName);

            return RedirectToAction("Index", "Students");
        }

        // GET: /Account/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
                return RedirectToAction("Login");

            var student = await _context.Students.FindAsync(studentId.Value);
            if (student == null)
                return RedirectToAction("Login");

            return View(student);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(int id, string email, string newPassword, string confirmPassword)
        {
            var sessionId = HttpContext.Session.GetInt32("StudentId");
            if (sessionId == null || sessionId.Value != id)
                return RedirectToAction("Login");

            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return RedirectToAction("Login");

            student.Email = email;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "New password and confirm password do not match.");
                    return View(student);
                }

                student.PasswordHash = HashPassword(newPassword);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index", "Students");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Helper: hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
