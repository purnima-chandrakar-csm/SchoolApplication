using HomeEF;
using MasterEF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DashboardApp.Services
{
    public interface IAdminApiService
    {
        Task<List<Teacher>> GetTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<bool> CreateTeacherAsync(Teacher teacher);

        Task<List<Student>> GetStudentsAsync();

        Task<Student?> GetStudentByIdAsync(int id);

        Task<bool> CreateStudentAsync(Student student);
    }
}
