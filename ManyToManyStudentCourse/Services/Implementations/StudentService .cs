using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Data.Entities;
using ManyToManyStudentCourse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManyToManyStudentCourse.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;
        public StudentService(AppDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            if (student == null)
            {
                _logger.LogError("Student object was null during creation.");
                throw new ArgumentNullException(nameof(student));
            }
            _logger.LogInformation($"Creating student: {student.FirstName} {student.LastName}");
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateStudentCoursesAsync(int studentId, List<int> selectedCourseIds)
        {
            var student = await _context.Students.Include(s => s.StudentCourses)
                                                 .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null) throw new Exception("Student not found");

            // Clear all existing courses for the student
            student.StudentCourses.Clear();

            // Add the selected courses
            foreach (var courseId in selectedCourseIds)
            {
                student.StudentCourses.Add(new StudentCourse { StudentId = studentId, CourseId = courseId });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> SearchStudent(string searchBox)
        {
            var students = _context.Students
                           .Where(s => s.LastName.Contains(searchBox) || s.FirstName.Contains(searchBox));
            return await students.ToListAsync();
        }

    }
}
