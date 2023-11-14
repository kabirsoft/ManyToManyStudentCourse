using System.ComponentModel.DataAnnotations.Schema;

namespace ManyToManyStudentCourse.Data.Entities
{
    public class StudentCourse
    {
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
