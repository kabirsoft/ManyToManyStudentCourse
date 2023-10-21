using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Data.Entities;

namespace ManyToManyStudentCourse.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int? id);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
        Task<Course?> GetCourseWithStudentsAsync(int? courseId);

    }
}
