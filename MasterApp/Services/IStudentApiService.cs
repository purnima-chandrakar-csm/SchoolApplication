using FileStorage;
using MasterEF;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterApp.Services
{
    public interface IStudentApiService
    {
        Task<List<Student>> GetStudentsAsync();

        Task<Student?> GetStudentByIdAsync(int id);

        Task<bool> CreateStudentAsync(Student student);

        Task<(bool Success, string ErrorMessage)> UpdateStudentAsync(Student student);

        Task<(bool Success, string ErrorMessage)> DeleteStudentAsync(int id);
        Task<MessageEF> UploadFileAsync(MyFileRequest request);
        Task<(byte[] Content, string ContentType, string FileName, bool Success)> DownloadFileAsync(string path);
    }
}

