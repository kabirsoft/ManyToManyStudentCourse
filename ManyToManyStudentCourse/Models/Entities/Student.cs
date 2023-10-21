using ManyToManyStudentCourse.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ManyToManyStudentCourse.Data
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set;}
        public string LastName { get; set;}

        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
    