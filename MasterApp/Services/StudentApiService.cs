using FileStorage;
using HttpRequests;
using MasterEF;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MasterApp.Services
{
    public class StudentApiService : IStudentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpRequest _httpRequest;

        public StudentApiService(
            IHttpClientFactory httpClientFactory,
            IHttpRequest httpRequest)
        {
            _httpClient = httpClientFactory.CreateClient("StudentApiClient");
            _httpRequest = httpRequest;
        }

        // GET: Student/GetStudent
        public async Task<List<Student>> GetStudentsAsync()
        {
            string jsonResponse =
                await _httpRequest.GetRequestAsync(
                    "Student/GetStudent",
                    null);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return new List<Student>();
            }

            var students =
                JsonConvert.DeserializeObject<List<Student>>(jsonResponse);

            return students ?? new List<Student>();
        }

        // GET: Student/GetStudentById/{id}
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            string jsonResponse =
                await _httpRequest.GetRequestAsync(
                    $"Student/GetStudentById/{id}",
                    null);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Student>(jsonResponse);
        }

        // POST: Student/CreateStudent
        public async Task<bool> CreateStudentAsync(Student student)
        {
            string parameterValues =
                JsonConvert.SerializeObject(student);

            string jsonResponse =
                await _httpRequest.PostRequestAsync(
                    "Student/CreateStudent",
                    parameterValues);

            return jsonResponse != null;
        }

        // PUT: Student/UpdateStudent
        public async Task<(bool Success, string ErrorMessage)> UpdateStudentAsync(
            Student student)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    "Student/UpdateStudent",
                    student);

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }

            string errorMessage =
                await response.Content.ReadAsStringAsync();

            return (false, errorMessage);
        }

        // DELETE: Student/DeleteStudent/{id}
        public async Task<(bool Success, string ErrorMessage)> DeleteStudentAsync(
            int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"Student/DeleteStudent/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }

            string errorMessage =
                await response.Content.ReadAsStringAsync();

            return (false, errorMessage);
        }

        public async Task<MessageEF> UploadFileAsync(MyFileRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "File/SaveFileToLocaBase64",
                request);

            string body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return new MessageEF
                {
                    Msg = response.IsSuccessStatusCode ? "Success" : "Upload failed",
                    Satus = response.IsSuccessStatusCode ? "True" : "False"
                };
            }

            try
            {
                return JsonConvert.DeserializeObject<MessageEF>(body)
                    ?? new MessageEF { Msg = body, Satus = "False" };
            }
            catch
            {
                return new MessageEF { Msg = body, Satus = "False" };
            }
        }

        public async Task<(byte[] Content, string ContentType, string FileName, bool Success)> DownloadFileAsync(
            string path)
        {
            var response = await _httpClient.GetAsync(
                $"File/DownloadFileFromLocal?path={System.Uri.EscapeDataString(path)}");

            byte[] content = await response.Content.ReadAsByteArrayAsync();
            string contentType = response.Content.Headers.ContentType?.ToString()
                ?? "application/octet-stream";
            string fileName = GetDownloadFileName(response.Content.Headers.ContentDisposition)
                ?? "download";

            return (content, contentType, fileName, response.IsSuccessStatusCode);
        }

        private static string GetDownloadFileName(ContentDispositionHeaderValue contentDisposition)
        {
            if (contentDisposition == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(contentDisposition.FileNameStar))
            {
                return contentDisposition.FileNameStar.Trim('"');
            }

            if (!string.IsNullOrWhiteSpace(contentDisposition.FileName))
            {
                return contentDisposition.FileName.Trim('"');
            }

            return null;
        }
    }
}