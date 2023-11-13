using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Models.ViewModels;
using ManyToManyStudentCourse.Services.Implementations;
using ManyToManyStudentCourse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManyToManyStudentCourse.Controllers
{
    public class StudentsController : Controller
    {
        public readonly IStudentService _studentService;
            private readonly ICourseService _courseService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ICourseService courseService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _courseService = courseService;
            _logger = logger;
        }
        // GET: Students
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all students...");
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            var allCourses = await _courseService.GetAllCoursesAsync();

            var viewModel = new StudentViewModel
            {
                Courses = allCourses.Select(c => new CourseViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Students/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                //-------- get the error message in log ---------

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        // Log or print the error for debugging
                        _logger.LogError($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }

                //--------- end -----------

                return View(viewModel);
            }
            var student = new Student
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
            };

            var createdStudent = await _studentService.CreateStudentAsync(student);

            var selectedCourseIds = viewModel.Courses.Where(c => c.IsSelected).Select(c => c.CourseId).ToList();

            // Now use the selectedCourseIds list to associate the courses with the newly created student
            await _studentService.UpdateStudentCoursesAsync(createdStudent.StudentId, selectedCourseIds);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        // GET: Students/Edit/5      
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            var allCourses = await _courseService.GetAllCoursesAsync();

            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentViewModel
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                RowVersion = student.RowVersion,
                Courses = allCourses.Select(c => new CourseViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    IsSelected = student.StudentCourses.Any(sc => sc.CourseId == c.CourseId)
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel viewModel)
        {
            if (id != viewModel.StudentId)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var student = new Student
            {
                StudentId = viewModel.StudentId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                RowVersion = viewModel.RowVersion,
            };
            await _studentService.UpdateStudentAsync(student);

            var selectedCourseIds = viewModel.Courses.Where(c => c.IsSelected).Select(c => c.CourseId).ToList();

            // Now use the selectedCourseIds list to add or remove the student's courses
            await _studentService.UpdateStudentCoursesAsync(viewModel.StudentId, selectedCourseIds);

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Details/5        
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: Students/Delete/5        
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
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
            await _studentService.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SearchStudent(string searchBox)
        {
            var Students = await _studentService.SearchStudent(searchBox);
            return View("Index", Students);
        }
    }
}
