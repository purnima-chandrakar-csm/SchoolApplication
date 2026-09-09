using HomeEF;
using HttpRequests;
using MasterEF;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DashboardApp.Services
{
    public class AdminApiService: IAdminApiService
    {
        private readonly IHttpRequest _httpRequest;

        public AdminApiService(IHttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }



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

        public async Task<Teacher> GetTeacherByIdAsync(int id)
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

        public async Task<Student> GetStudentByIdAsync(int id)
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

      

    }
}
