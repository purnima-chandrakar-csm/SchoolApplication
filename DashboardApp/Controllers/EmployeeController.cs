using DashboardApp.Services;
using MasterEF;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DashboardApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IAdminApiService _adminApiService;

        public EmployeeController(IAdminApiService adminApiService)
        {
            _adminApiService = adminApiService;
        }

        // GET: /Employee
        // GET: /Employee/Index
        public async Task<IActionResult> Index()
        {
            var students = await _adminApiService.GetStudentsAsync();

            return View(students);
        }

        // =========================
        // CREATE STUDENT
        // =========================

        // GET: /Employee/CreateStudent
        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View();
        }

        // POST: /Employee/CreateStudent
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

        // =========================
        // VIEW STUDENT
        // =========================

        // GET: /Employee/ViewStudent/5
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
