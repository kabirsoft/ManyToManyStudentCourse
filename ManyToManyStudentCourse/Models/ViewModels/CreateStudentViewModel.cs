namespace ManyToManyStudentCourse.Models.ViewModels
{
    public class CreateStudentViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<CourseViewModel> Courses { get; set; } = new List<CourseViewModel>();
    }
}
