using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentRegistrationApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private void CheckLogin()
        {
            if (HttpContext.Session.GetInt32("StudentId") == null)
            {
                Response.Redirect("/Account/Login");
            }
        }

        // ========= DASHBOARD / STUDENT HOME =========
        public async Task<IActionResult> Index()
        {
            CheckLogin();

            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null || studentId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Load ONLY the logged-in student with their enrolled courses
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // "My next lessons" = only this student's enrolled courses
            var myNextLessons = student.StudentCourses
                .Select(sc => sc.Course)
                .OrderBy(c => c.StartDate)
                .ToList();

            ViewBag.MyNextLessons = myNextLessons;

            // Optional – if you use this in the view
            ViewBag.TotalCourses = myNextLessons.Count;

            // View now uses a single Student model (the logged-in student)
            return View(student);
        }

        // ========= ADMIN / CREATE STUDENT (IF YOU USE THIS) =========
        public async Task<IActionResult> Create()
        {
            ViewBag.Courses = new MultiSelectList(
                await _context.Courses.ToListAsync(),
                "Id",
                "CourseName"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, int[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                student.RegistrationNumber = await GenerateNextRegistrationNumberAsync();
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                if (selectedCourses != null && selectedCourses.Length > 0)
                {
                    foreach (var courseId in selectedCourses)
                    {
                        _context.StudentCourses.Add(new StudentCourse
                        {
                            StudentId = student.Id,
                            CourseId = courseId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Courses = new MultiSelectList(
                await _context.Courses.ToListAsync(),
                "Id",
                "CourseName",
                selectedCourses
            );
            return View(student);
        }

        // ========= REG NO GENERATION =========
        private async Task<string> GenerateNextRegistrationNumberAsync()
        {
            // Desired format: STU-1001, STU-1002, ...
            const int start = 1001;
            var last = await _context.Students
                .Where(s => s.RegistrationNumber != null && s.RegistrationNumber.StartsWith("STU-"))
                .OrderByDescending(s => s.RegistrationNumber)
                .Select(s => s.RegistrationNumber)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(last))
            {
                var parts = last.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out var n))
                {
                    return $"STU-{n + 1}";
                }
            }

            // Fallback: scan any STU digits
            var alt = await _context.Students
                .Where(s => s.RegistrationNumber != null && s.RegistrationNumber.StartsWith("STU"))
                .Select(s => s.RegistrationNumber)
                .ToListAsync();

            var max = 0;
            foreach (var r in alt)
            {
                var digits = new string(r.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out var v)) max = Math.Max(max, v);
            }

            if (max >= start) return $"STU-{max + 1}";
            return $"STU-{start}";
        }

        // ========= DETAILS =========
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();
            return View(student);
        }
    }
}
