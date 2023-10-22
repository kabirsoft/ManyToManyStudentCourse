namespace ManyToManyStudentCourse.Models.ViewModels
{
    public class StudentViewModel
    {       
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] RowVersion { get; set; }
        public List<CourseViewModel> Courses { get; set; } = new List<CourseViewModel>();
    }
}
