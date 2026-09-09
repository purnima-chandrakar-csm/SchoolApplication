using HomeEF;
using HomeApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IApiService _apiService;

        public TeacherController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Teacher
        public async Task<IActionResult> Index()
        {
            var teachers = await _apiService.GetTeachersAsync();

            if (teachers == null || teachers.Count == 0)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to fetch teachers from the API. Please ensure the API is running.");
            }

            return View(teachers);
        }

        // GET: Teacher/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            bool success =
                await _apiService.CreateTeacherAsync(teacher);

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the teacher record.");

            return View(teacher);
        }

        // GET: Teacher/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var teacher =
                await _apiService.GetTeacherByIdAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teacher/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            var result =
                await _apiService.UpdateTeacherAsync(teacher);

            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                result.ErrorMessage);

            return View(teacher);
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result =
                await _apiService.DeleteTeacherAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] =
                    result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}