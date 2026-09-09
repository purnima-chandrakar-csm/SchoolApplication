using MasterEF;
using MasterApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MasterApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentApiService _studentApiService;

        public StudentController(IStudentApiService studentApiService)
        {
            _studentApiService = studentApiService;
        }

        // GET: Student
        public async Task<IActionResult> Index()
        {
            var students =
                await _studentApiService.GetStudentsAsync();

            if (students == null || students.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to fetch students from the API. Please ensure the API is running.");
            }

            return View(students);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            bool success =
                await _studentApiService.CreateStudentAsync(student);

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the student record.");

            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student =
                await _studentApiService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var result =
                await _studentApiService.UpdateStudentAsync(student);

            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                result.ErrorMessage);

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result =
                await _studentApiService.DeleteStudentAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] =
                    result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
