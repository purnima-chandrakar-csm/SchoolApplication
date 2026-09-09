using MasterEF;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterApi.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentAsync();
        Task<Student> GetStudentAsync(int id);

        Task<int> CreateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);

        Task<bool> UpdateStudentAsync(Student student);
    }
}
