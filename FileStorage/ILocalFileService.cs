using System.Threading.Tasks;

namespace FileStorage
{
    public interface ILocalFileService
    {
        Task<MessageEF> SaveFileToLocaBase64(MyFileRequest myFileRequest);
        Task<MyFileResult> DownloadFileFromLocal(string path);
    }
}
