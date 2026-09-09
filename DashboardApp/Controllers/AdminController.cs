using DashboardApp.Services;
using DashboardApp.ViewModels;
using HomeEF;
using MasterEF;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DashboardApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminApiService _adminApiService;

        public AdminController(IAdminApiService adminApiService)
        {
            _adminApiService = adminApiService;
        }

        // GET: /Admin
        // GET: /Admin/Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                Teachers = await _adminApiService.GetTeachersAsync(),
                Students = await _adminApiService.GetStudentsAsync()
            };

            return View(viewModel);
        }

        // =========================
        // TEACHER
        // =========================

        // GET: /Admin/CreateTeacher
        [HttpGet]
        public IActionResult CreateTeacher()
        {
            return View();
        }

        // POST: /Admin/CreateTeacher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            bool result = await _adminApiService.CreateTeacherAsync(teacher);

            if (result)
            {
                TempData["SuccessMessage"] = "Teacher created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Unable to create teacher.");

            return View(teacher);
        }

        // GET: /Admin/ViewTeacher/5
        [HttpGet]
        public async Task<IActionResult> ViewTeacher(int id)
        {
            var teacher = await _adminApiService.GetTeacherByIdAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // =========================
        // STUDENT
        // =========================

        // GET: /Admin/CreateStudent
        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View();
        }

        // POST: /Admin/CreateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            bool result = await _adminApiService.CreateStudentAsync(student);

            if (result)
            {
                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Unable to create student.");

            return View(student);
        }

        // GET: /Admin/ViewStudent/5
        [HttpGet]
        public async Task<IActionResult> ViewStudent(int id)
        {
            var student = await _adminApiService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
    }
}
