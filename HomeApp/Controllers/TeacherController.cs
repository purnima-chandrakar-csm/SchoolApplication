using FileStorage;
using HomeApp.Services;
using HomeEF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HomeApp.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILocalFileService _localFileService; // Injected for crash dump file saving
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(IApiService apiService, ILocalFileService localFileService,
            ILogger<TeacherController> logger)
        {
            _apiService = apiService;
            _localFileService = localFileService;
            _logger = logger;
        }

        // GET: Teacher
        public async Task<IActionResult> Index()
        {
            try
            {
                var teachers = await _apiService.GetTeachersAsync();

                if (teachers == null || teachers.Count == 0)
                {
                    _logger.LogWarning("GetTeachersAsync returned no teachers data or was null.");
                    ModelState.AddModelError(
                        "",
                        "Unable to fetch teachers from the API. Please ensure the API is running.");
                }

                return View(teachers);
            }
            catch (Exception ex)
            {
                await DumpErrorToFileAsync(ex, "Index");
                ModelState.AddModelError("", "An unexpected error occurred while loading teachers.");
                return View();
            }
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

        private async Task DumpErrorToFileAsync(Exception ex, string contextIdentifier)
        {
            // 1. Log cleanly to global background logging engine systems (like Serilog or Console)
            _logger.LogError(ex, "An unhandled exception occurred in TeacherController during context: {Context}", contextIdentifier);

            // 2. Format a structured text report and feed it into LocalFileService to store inside a dynamic error directory
            try
            {
                var builder = new System.Text.StringBuilder();
                builder.AppendLine($"=========================================================================");
                builder.AppendLine($"CRASH TIMESTAMP : {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                builder.AppendLine($"CONTROLLER      : TeacherController");
                builder.AppendLine($"CONTEXT ACTION  : {contextIdentifier}");
                builder.AppendLine($"ERROR MESSAGE   : {ex.Message}");
                builder.AppendLine($"=========================================================================");
                builder.AppendLine($"STACK TRACE     :");
                builder.AppendLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    builder.AppendLine($"-------------------------------------------------------------------------");
                    builder.AppendLine($"INNER EXCEPTION : {ex.InnerException.Message}");
                    builder.AppendLine(ex.InnerException.StackTrace);
                }

                byte[] logBytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
                string uniqueLogFileName = $"Error_{DateTime.Now:yyyyMMdd_HHmmss}_{contextIdentifier}_{Guid.NewGuid():N}.txt";

                // Streams payload directly to LocalFileService using your base64 file strategy pattern
                await _localFileService.SaveFileToLocaBase64(new MyFileRequest
                {
                    FolderName = "SystemErrors", // Will create a subfolder called "SystemErrors" under your configured LocalRootDir
                    FileName = uniqueLogFileName,
                    FileContenctBase64 = Convert.ToBase64String(logBytes)
                });
            }
            catch (Exception loggingEx)
            {
                // Fallback catch to prevent a logging failure from breaking or interrupting initial application pipelines
                System.Diagnostics.Trace.WriteLine($"Failed to write standalone text crash log: {loggingEx.Message}");
            }
        }

    }
}