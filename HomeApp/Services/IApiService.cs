using HomeEF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeApp.Services
{
    public interface IApiService
    {
        Task<List<Teacher>> GetTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<bool> CreateTeacherAsync(Teacher teacher);
        Task<(bool Success, string ErrorMessage)> UpdateTeacherAsync(Teacher teacher);
        Task<(bool Success, string ErrorMessage)> DeleteTeacherAsync(int id);
    }
}