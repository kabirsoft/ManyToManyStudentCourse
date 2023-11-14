using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Data.Entities;
using ManyToManyStudentCourse.Services.Implementations;
using ManyToManyStudentCourse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManyToManyStudentCourse.Controllers
{
    public class CoursesController : Controller
    {
        public readonly ICourseService _courseService;
        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (course == null)
            {
                return BadRequest("Course data is required");
            }

            var createdCourse = await _courseService.CreateCourseAsync(course);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // GET: Courses/Edit/5       
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync((int)id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            try
            {
                await _courseService.UpdateCourseAsync(course);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _courseService.GetCourseByIdAsync(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Details/5        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _courseService.GetCourseWithStudentsAsync(id.Value);

            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // GET: Students/Delete/5        
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _courseService.GetCourseByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
