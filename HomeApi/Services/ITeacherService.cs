using HomeEF;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeApi.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllTeacherAsync();
        Task<Teacher> GetTeacherAsync(int id);

        Task<int> CreateTeacherAsync(Teacher teacher);
        Task<bool> DeleteTeacherAsync(int id);

        Task<bool> UpdateTeacherAsync(Teacher teacher);
    }
}
