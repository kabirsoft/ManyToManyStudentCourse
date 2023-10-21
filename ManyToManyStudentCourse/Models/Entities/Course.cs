using System.ComponentModel.DataAnnotations;

namespace ManyToManyStudentCourse.Data.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public double CreditHour { get; set; }      
        public virtual ICollection<StudentCourse> StudentCourse { get; set; } = new List<StudentCourse>();
    }
}
