using HttpRequests;
using MasterEF;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
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
    }
}