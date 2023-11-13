using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManyToManyStudentCourse.Controllers;
using ManyToManyStudentCourse.Data;
using ManyToManyStudentCourse.Data.Entities;
using ManyToManyStudentCourse.Models.ViewModels;
using ManyToManyStudentCourse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ManyToManyStudentCourse.Tests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly Mock<ICourseService> _mockCourseService;
            private readonly Mock<ILogger<StudentsController>> _mockLogger;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _mockStudentService = new Mock<IStudentService>();
            _mockCourseService = new Mock<ICourseService>();
                _mockLogger = new Mock<ILogger<StudentsController>>();
            _controller = new StudentsController(_mockStudentService.Object, _mockCourseService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithListOfStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { StudentId = 1, FirstName = "John", LastName = "Doe" },
                new Student { StudentId = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _mockStudentService.Setup(s => s.GetAllStudentsAsync()).ReturnsAsync(students);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        // ---------- Add for Create ------------------

        [Fact]
        public async Task Create_Get_ReturnsViewWithListOfAllCourses()
        {
            // Arrange
            var courses = new List<Course>
            {
                new Course { CourseId = 1, Title = "Math" },
                new Course { CourseId = 2, Title = "English" }
            };

            _mockCourseService.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(courses);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StudentViewModel>(viewResult.Model);
            Assert.Equal(2, model.Courses.Count);
        }

        [Fact]
        public async Task Create_Post_ValidInput_RedirectsToIndex()
        {
            // Arrange
            var studentViewModel = new CreateStudentViewModel
            {
                FirstName = "John",
                LastName = "Doe",
                Courses = new List<CourseViewModel>()
            };

            _mockStudentService.Setup(s => s.CreateStudentAsync(It.IsAny<Student>())).ReturnsAsync(new Student { StudentId = 1, FirstName = "John", LastName = "Doe" });

            // Act
            var result = await _controller.Create(studentViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_InvalidInput_ReturnsSameView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Some error");
            var studentViewModel = new CreateStudentViewModel();

            // Act
            var result = await _controller.Create(studentViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(studentViewModel, viewResult.Model);
        }

        //------- end for Create ----------

        // --------- start for Edit ---------
        [Fact]
        public async Task Edit_Get_ValidId_ReturnsViewWithStudentDetailsAndListOfAllCourses()
        {
            // Arrange
            int studentId = 1;
            var student = new Student { StudentId = studentId, FirstName = "John", LastName = "Doe" };
            var courses = new List<Course>
            {
                new Course { CourseId = 1, Title = "Course 1" },
                new Course { CourseId = 2, Title = "Course 2" }
            };

            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync(student);
            _mockCourseService.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(courses);

            // Act
            var result = await _controller.Edit(studentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StudentViewModel>(viewResult.Model);
            Assert.Equal(studentId, model.StudentId);
            Assert.Equal(2, model.Courses.Count);
        }

        [Fact]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int studentId = 1;
            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync((Student)null);

            // Act
            var result = await _controller.Edit(studentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidInput_RedirectsToIndex()
        {
            // Arrange
            var studentViewModel = new StudentViewModel
            {
                StudentId = 1,
                FirstName = "John",
                LastName = "Doe",
                Courses = new List<CourseViewModel>()
            };

            var updatedStudent = new Student
            {
                StudentId = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockStudentService.Setup(s => s.UpdateStudentAsync(It.IsAny<Student>())).ReturnsAsync(updatedStudent);
            _mockStudentService.Setup(s => s.UpdateStudentCoursesAsync(It.IsAny<int>(), It.IsAny<List<int>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(studentViewModel.StudentId, studentViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
        [Fact]
        public async Task Edit_Post_InvalidInput_ReturnsSameView()
        {
            // Arrange
            var studentViewModel = new StudentViewModel();
            _controller.ModelState.AddModelError("Error", "Some error");

            // Act
            var result = await _controller.Edit(studentViewModel.StudentId, studentViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(studentViewModel, viewResult.Model);
        }

        // --------- End for Edit ---------

        //----------- start for delete -----
        [Fact]
        public async Task Delete_Get_ExistingStudent_ReturnsViewWithStudent()
        {
            // Arrange
            var studentId = 1;
            var student = new Student { StudentId = studentId, FirstName = "John", LastName = "Doe" };

            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync(student);

            // Act
            var result = await _controller.Delete(studentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Student>(viewResult.ViewData.Model);
            Assert.Equal(studentId, model.StudentId);
        }

        [Fact]
        public async Task Delete_Get_NonExistingStudent_ReturnsNotFound()
        {
            // Arrange
            var studentId = 1;

            _mockStudentService.Setup(s => s.GetStudentByIdAsync(studentId)).ReturnsAsync((Student)null);

            // Act
            var result = await _controller.Delete(studentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_Post_ValidStudent_RedirectsToIndex()
        {
            // Arrange
            var studentId = 1;

            _mockStudentService.Setup(s => s.DeleteStudentAsync(studentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(studentId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        //----------- end delete ----------

        // start for search student ------
        [Fact]
        public async Task SearchStudent_ValidSearch_ReturnsViewWithStudents()
        {
            // Arrange
            var searchBox = "Doe";
            var students = new List<Student>
            {
                new Student { StudentId = 1, FirstName = "John", LastName = "Doe" },
                new Student { StudentId = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _mockStudentService.Setup(s => s.SearchStudent(searchBox)).ReturnsAsync(students);

            // Act
            var result = await _controller.SearchStudent(searchBox);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
            Assert.Equal("Index", viewResult.ViewName);
        }

        [Fact]
        public async Task SearchStudent_NoMatch_ReturnsViewWithNoStudents()
        {
            // Arrange
            var searchBox = "NonExistingName";
            var students = new List<Student>(); // Empty list

            _mockStudentService.Setup(s => s.SearchStudent(searchBox)).ReturnsAsync(students);

            // Act
            var result = await _controller.SearchStudent(searchBox);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Student>>(viewResult.ViewData.Model);
            Assert.Empty(model);
            Assert.Equal("Index", viewResult.ViewName);
        }


        // end search student ----------

    }
}
