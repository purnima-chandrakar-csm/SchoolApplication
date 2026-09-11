using FileStorage;
using HomeEF;
using HomeApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> Create(Teacher teacher, IFormFile document)
        {
            if (!ModelState.IsValid)
            {
                return View(teacher);
            }

            bool success =
                await _apiService.CreateTeacherAsync(teacher);

            if (success)
            {
                await TryUploadDocumentAsync(document, "Teachers");
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the teacher record.");

            return View(teacher);
        }

        public async Task<IActionResult> Download(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return NotFound();
            }

            var result = await _apiService.DownloadFileAsync(path);
            if (!result.Success || result.Content == null || result.Content.Length == 0)
            {
                return NotFound();
            }

            return File(result.Content, result.ContentType, result.FileName);
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

        private async Task TryUploadDocumentAsync(IFormFile document, string folderName)
        {
            if (document == null || document.Length == 0)
            {
                return;
            }

            using var memoryStream = new MemoryStream();
            await document.CopyToAsync(memoryStream);
            string uniqueName = $"{Guid.NewGuid():N}_{Path.GetFileName(document.FileName)}";

            MessageEF uploadResult = await _apiService.UploadFileAsync(new MyFileRequest
            {
                FolderName = folderName,
                FileName = uniqueName,
                FileContenctBase64 = Convert.ToBase64String(memoryStream.ToArray())
            });

            if (uploadResult.Satus == "True")
            {
                TempData["UploadedPath"] = $"{folderName}/{uniqueName}";
                TempData["SuccessMessage"] = "Teacher created and file uploaded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Teacher was created, but the file could not be uploaded: " + uploadResult.Msg;
            }
        }
    }
}