using System.IO;

namespace FileStorage
{
    public class MyFileResult
    {
        public Stream Filestream { get; set; }
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
        public string Status { get; set; } = "False";
    }
}
