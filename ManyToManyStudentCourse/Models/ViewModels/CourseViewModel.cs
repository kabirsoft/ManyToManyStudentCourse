namespace ManyToManyStudentCourse.Models.ViewModels
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public double CreditHour { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
