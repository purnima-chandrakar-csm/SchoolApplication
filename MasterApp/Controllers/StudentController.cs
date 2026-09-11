using FileStorage;
using MasterEF;
using MasterApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Create(Student student, IFormFile document)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            bool success =
                await _studentApiService.CreateStudentAsync(student);

            if (success)
            {
                await TryUploadDocumentAsync(document, "Students");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the student record.");

            return View(student);
        }

        public async Task<IActionResult> Download(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return NotFound();
            }

            var result = await _studentApiService.DownloadFileAsync(path);
            if (!result.Success || result.Content == null || result.Content.Length == 0)
            {
                return NotFound();
            }

            return File(result.Content, result.ContentType, result.FileName);
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

        private async Task TryUploadDocumentAsync(IFormFile document, string folderName)
        {
            if (document == null || document.Length == 0)
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            await document.CopyToAsync(memoryStream);
            string uniqueName = $"{Guid.NewGuid():N}_{Path.GetFileName(document.FileName)}";

            MessageEF uploadResult = await _studentApiService.UploadFileAsync(new MyFileRequest
            {
                FolderName = folderName,
                FileName = uniqueName,
                FileContenctBase64 = Convert.ToBase64String(memoryStream.ToArray())
            });

            if (uploadResult.Satus == "True")
            {
                TempData["UploadedPath"] = $"{folderName}/{uniqueName}";
                TempData["SuccessMessage"] = "Student created and file uploaded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Student was created, but the file could not be uploaded: " + uploadResult.Msg;
            }
        }
    }
}
