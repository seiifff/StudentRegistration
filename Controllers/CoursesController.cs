using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StudentRegistrationApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- GET: Courses/Index ----------------
        public async Task<IActionResult> Index()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null || studentId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = await _context.Courses.ToListAsync();

            var enrolledIds = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId.Value)
                .Select(sc => sc.CourseId)
                .ToListAsync();

            ViewBag.StudentId = studentId.Value;
            ViewBag.EnrolledIds = enrolledIds;

            return View(courses);
        }

        // -------- Optional legacy Enroll page (kept as is) --------
        public async Task<IActionResult> Enroll(int? studentId)
        {
            var sid = studentId ?? HttpContext.Session.GetInt32("StudentId");

            if (sid == null || sid == 0)
                return RedirectToAction("Login", "Account");

            ViewBag.Courses = await _context.Courses.ToListAsync();

            return View(sid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int studentId, int[] selectedCourses)
        {
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            if (selectedCourses == null || selectedCourses.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one course.");
                ViewBag.Courses = await _context.Courses.ToListAsync();
                return View(studentId);
            }

            var existing = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => sc.CourseId)
                .ToListAsync();

            foreach (var cId in selectedCourses)
            {
                if (!existing.Contains(cId))
                {
                    _context.StudentCourses.Add(new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = cId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment saved successfully!";
            return RedirectToAction("UpcomingClasses");
        }

        // ------------- POST: EnrollSingle (from cards) -------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollSingle(int studentId, int courseId)
        {
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var exists = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (!exists)
            {
                _context.StudentCourses.Add(new StudentCourse
                {
                    StudentId = studentId,
                    CourseId = courseId
                });

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course enrolled successfully!";
            }
            else
            {
                TempData["SuccessMessage"] = "You are already enrolled in this course.";
            }

            return RedirectToAction("Index");
        }

        // ------------- POST: DisenrollSingle (from cards) -------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnenrollSingle(int studentId, int courseId)
        {
            if (studentId == 0)
                return RedirectToAction("Login", "Account");

            var record = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (record != null)
            {
                _context.StudentCourses.Remove(record);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "You have been disenrolled from the course.";
            }
            else
            {
                TempData["SuccessMessage"] = "You are not enrolled in this course.";
            }

            return RedirectToAction("Index");
        }

        // ------------- GET: UpcomingClasses -------------
        public async Task<IActionResult> UpcomingClasses()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login", "Account");

            var upcoming = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => new
                {
                    sc.Course.CourseName,
                    sc.Course.StartDate,
                    LecturerName = sc.Course.Lecturer
                })
                .ToListAsync();

            return View(upcoming);
        }
    }
}
