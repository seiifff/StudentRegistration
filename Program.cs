using Microsoft.EntityFrameworkCore;
using StudentRegistrationApp.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Use your SQL Express instance + StudentRegistrationDb
var connectionString =
    "Server=ATHEEK\\SQLEXPRESS;Database=StudentRegistrationDb;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// ✅ Register EF Core DbContext with SQL Express connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// 🔐 Helper function to hash passwords
string HashPassword(string password)
{
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

// ✅ Auto-create schema & seed data (DB itself must already exist)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // This will create tables if they don't exist
    db.Database.EnsureCreated();

    // Seed courses
    if (!db.Courses.Any())
    {
        db.Courses.AddRange(
            new Course { CourseCode = "CS101", CourseName = "Introduction to Computer Science", Lecturer = "Dr. Smith", StartDate = DateTime.Now.AddDays(7) },
            new Course { CourseCode = "WD201", CourseName = "Web Development Fundamentals", Lecturer = "Prof. Johnson", StartDate = DateTime.Now.AddDays(14) },
            new Course { CourseCode = "UI301", CourseName = "UI/UX Design Principles", Lecturer = "Ms. Williams", StartDate = DateTime.Now.AddDays(21) },
            new Course { CourseCode = "DB101", CourseName = "Database Design & SQL", Lecturer = "Dr. Brown", StartDate = DateTime.Now.AddDays(10) },
            new Course { CourseCode = "PY101", CourseName = "Python Programming Beginner", Lecturer = "Prof. Davis", StartDate = DateTime.Now.AddDays(5) },
            new Course { CourseCode = "AI201", CourseName = "Introduction to Artificial Intelligence", Lecturer = "Dr. Miller", StartDate = DateTime.Now.AddDays(30) },
            new Course { CourseCode = "NET101", CourseName = "C# .NET Fundamentals", Lecturer = "Prof. Wilson", StartDate = DateTime.Now.AddDays(12) },
            new Course { CourseCode = "MG101", CourseName = "Business Management Basics", Lecturer = "Ms. Taylor", StartDate = DateTime.Now.AddDays(15) }
        );

        db.SaveChanges();
    }

    // Seed sample students with course enrollments
    if (!db.Students.Any())
    {
        var courses = db.Courses.ToList();
        var random = new Random();

        var students = new List<Student>
        {
            new Student { FullName = "Alice Johnson", Email = "alice.johnson@email.com", RegistrationNumber = "STU-1001", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Bob Smith", Email = "bob.smith@email.com", RegistrationNumber = "STU-1002", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Carol Davis", Email = "carol.davis@email.com", RegistrationNumber = "STU-1003", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "David Wilson", Email = "david.wilson@email.com", RegistrationNumber = "STU-1004", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Emma Brown", Email = "emma.brown@email.com", RegistrationNumber = "STU-1005", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Frank Miller", Email = "frank.miller@email.com", RegistrationNumber = "STU-1006", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Grace Lee", Email = "grace.lee@email.com", RegistrationNumber = "STU-1007", PasswordHash = HashPassword("Password123!") },
            new Student { FullName = "Henry Taylor", Email = "henry.taylor@email.com", RegistrationNumber = "STU-1008", PasswordHash = HashPassword("Password123!") }
        };

        db.Students.AddRange(students);
        db.SaveChanges();

        // Enroll students in random courses
        foreach (var student in students)
        {
            int courseCount = random.Next(1, 4); // Each student gets 1-3 courses
            var selectedCourses = courses.OrderBy(x => random.Next()).Take(courseCount).ToList();

            foreach (var course in selectedCourses)
            {
                db.StudentCourses.Add(new StudentCourse
                {
                    StudentId = student.Id,
                    CourseId = course.Id
                });
            }
        }

        db.SaveChanges();
    }
}

// ✅ Only force HTTPS in Production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// In Development we just run on HTTP
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
