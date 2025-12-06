using System.ComponentModel.DataAnnotations;

namespace StudentRegistrationApp.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } // Represents the start date of the course

        [StringLength(100)]
        public string Lecturer { get; set; } = string.Empty; // Represents the lecturer's name

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
