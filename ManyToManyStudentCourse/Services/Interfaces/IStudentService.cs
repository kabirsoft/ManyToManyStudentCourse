using ManyToManyStudentCourse.Data;
using Microsoft.AspNetCore.Mvc;

namespace ManyToManyStudentCourse.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student> UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
        Task UpdateStudentCoursesAsync(int studentId, List<int> courseIds);
        Task<IEnumerable<Student>> SearchStudent(string searchBox);

    }
}
