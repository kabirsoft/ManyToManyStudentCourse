using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Data.Entities;
using ManyToManyStudentCourse.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace ManyToManyStudentCourse.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        public CourseService(AppDbContext context)
        {
                _context = context;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            if(course  == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int? id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course?> GetCourseWithStudentsAsync(int? courseId)
        {          
            return await _context.Courses.Include(c => c.StudentCourse)
            .ThenInclude(sc => sc.Student)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);            
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }
    }
}
