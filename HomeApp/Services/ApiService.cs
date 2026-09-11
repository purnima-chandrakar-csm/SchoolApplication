using FileStorage;
using HomeEF;
using HttpRequests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HomeApp.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpRequest _httpRequest;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpRequest httpRequest)
        {
            _httpClient = httpClientFactory.CreateClient("TeacherApiClient");
            _httpRequest = httpRequest;
        }

        // GET: Teacher/GetTeacher
        public async Task<List<Teacher>> GetTeachersAsync()
        {
            string jsonResponse = await _httpRequest.GetRequestAsync(
                "Teacher/GetTeacher",
                null);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return new List<Teacher>();
            }

            var teachers =
                JsonConvert.DeserializeObject<List<Teacher>>(jsonResponse);

            return teachers ?? new List<Teacher>();
        }

        // GET: Teacher/GetTeacherById/{id}
        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            string jsonResponse = await _httpRequest.GetRequestAsync(
                $"Teacher/GetTeacherById/{id}",
                null);

            if (string.IsNullOrEmpty(jsonResponse))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Teacher>(jsonResponse);
        }

        // POST: Teacher/CreateTeacher
        public async Task<bool> CreateTeacherAsync(Teacher teacher)
        {
            string parameterValues =
                JsonConvert.SerializeObject(teacher);

            string jsonResponse =
                await _httpRequest.PostRequestAsync(
                    "Teacher/CreateTeacher",
                    parameterValues);

            return jsonResponse != null;
        }

        // PUT: Teacher/UpdateTeacher
        public async Task<(bool Success, string ErrorMessage)> UpdateTeacherAsync(
            Teacher teacher)
        {
            var response = await _httpClient.PutAsJsonAsync(
                "Teacher/UpdateTeacher",
                teacher);

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }

            string errorMessage =
                await response.Content.ReadAsStringAsync();

            return (false, errorMessage);
        }

        // DELETE: Teacher/DeleteTeacher/{id}
        public async Task<(bool Success, string ErrorMessage)> DeleteTeacherAsync(
            int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"Teacher/DeleteTeacher/{id}");

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