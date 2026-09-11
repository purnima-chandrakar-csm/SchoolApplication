using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage
{
    public class LocalFileService : ILocalFileService
    {
        private readonly IConfiguration _configuration;

        public LocalFileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MessageEF> SaveFileToLocaBase64(MyFileRequest myFileRequest)
        {
            MessageEF msg = new MessageEF();
            try
            {
                if (myFileRequest == null
                    || string.IsNullOrWhiteSpace(myFileRequest.FileContenctBase64)
                    || string.IsNullOrWhiteSpace(myFileRequest.FileName)
                    || string.IsNullOrWhiteSpace(myFileRequest.FolderName))
                {
                    msg.Msg = "File name, folder name, and file content are required";
                    msg.Satus = "False";
                    return msg;
                }

                string rootDir = Convert.ToString(_configuration["AzurSettings:LocalRootDir"]);
                if (string.IsNullOrWhiteSpace(rootDir))
                {
                    msg.Msg = "Local root directory is not configured";
                    msg.Satus = "False";
                    return msg;
                }

                byte[] data = Convert.FromBase64String(myFileRequest.FileContenctBase64);
                string safeFileName = Path.GetFileName(myFileRequest.FileName);
                string targetDirectory = CombineUnderRoot(rootDir, myFileRequest.FolderName);
                if (targetDirectory == null)
                {
                    msg.Msg = "Invalid folder path";
                    msg.Satus = "False";
                    return msg;
                }

                myFileRequest.FolderName = targetDirectory;
                myFileRequest.FileName = safeFileName;

                if (_configuration["AzurSettings:AllowNewDirectory"] == "Yes")
                {
                    if (!Directory.Exists(myFileRequest.FolderName))
                    {
                        Directory.CreateDirectory(myFileRequest.FolderName);
                    }
                }

                if (Directory.Exists(myFileRequest.FolderName))
                {
                    string fullPath = Path.Combine(myFileRequest.FolderName, myFileRequest.FileName);
                    if (!IsPathUnderRoot(rootDir, fullPath))
                    {
                        msg.Msg = "Invalid file path";
                        msg.Satus = "False";
                        return msg;
                    }

                    if (File.Exists(fullPath))
                    {
                        msg.Msg = "File already exists with same name";
                        msg.Satus = "False";
                    }
                    else
                    {
                        await File.WriteAllBytesAsync(fullPath, data);
                        msg.Msg = "Success";
                        msg.Satus = "True";
                    }
                }
                else
                {
                    msg.Msg = "DIractory Not Found";
                    msg.Satus = "False";
                }
            }
            catch (Exception ex)
            {
                msg.Msg = ex.Message;
                msg.Satus = "False";
            }

            return msg;
        }

        public async Task<MyFileResult> DownloadFileFromLocal(string path)
        {
            MyFileResult fr = new MyFileResult();
            try
            {
                string rootDir = Convert.ToString(_configuration["AzurSettings:LocalRootDir"]);
                string folderName = "";
                string fileName = "";
                try
                {
                    folderName = getFolderOrFileName(path, "folder");
                    fileName = getFolderOrFileName(path, "file");
                }
                catch (Exception)
                {
                }

                if (folderName != "" && fileName != "")
                {
                    string replacedFolderName = CombineUnderRoot(rootDir, folderName);
                    if (replacedFolderName != null && Directory.Exists(replacedFolderName))
                    {
                        string fullPath = Path.Combine(replacedFolderName, Path.GetFileName(fileName));
                        if (!IsPathUnderRoot(rootDir, fullPath) || !File.Exists(fullPath))
                        {
                            return NotFoundFile();
                        }

                        Stream fs = File.OpenRead(fullPath);
                        fr.Filestream = fs;
                        fr.ContentType = GetMimeType(fullPath);
                        fr.FileName = Path.GetFileName(fileName);
                        fr.Status = "True";
                        await Task.CompletedTask;
                        return fr;
                    }

                    return NotFoundFile();
                }

                return NotFoundFile();
            }
            catch (Exception ex)
            {
                byte[] fileBytes = Encoding.ASCII.GetBytes(ex.Message);
                Stream textstream = new MemoryStream(fileBytes);
                fr.Filestream = textstream;
                fr.ContentType = "text/plain";
                fr.FileName = "File_Not_Found.txt";
                fr.Status = "False";
                return fr;
            }
        }

        private static MyFileResult NotFoundFile()
        {
            byte[] fileBytes = Convert.FromBase64String("RmlsZSBOb3QgZm91bmQ=");
            return new MyFileResult
            {
                Filestream = new MemoryStream(fileBytes),
                ContentType = "text/plain",
                FileName = "File_Not_Found.txt",
                Status = "False"
            };
        }

        private string getFolderOrFileName(string path, string type)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            string normalized = path.Replace("\\", "/").Trim().Trim('/');
            int lastSlash = normalized.LastIndexOf('/');
            if (lastSlash < 0)
            {
                return type == "file" ? Path.GetFileName(normalized) : "";
            }

            if (type == "file")
            {
                return Path.GetFileName(normalized.Substring(lastSlash + 1));
            }

            return normalized.Substring(0, lastSlash);
        }

        private static string CombineUnderRoot(string rootDir, string relativeFolder)
        {
            if (string.IsNullOrWhiteSpace(rootDir) || string.IsNullOrWhiteSpace(relativeFolder))
            {
                return null;
            }

            string cleaned = relativeFolder.Replace("/", "\\").Trim().TrimStart('\\');
            string combined = Path.GetFullPath(Path.Combine(rootDir, cleaned));
            if (!IsPathUnderRoot(rootDir, combined))
            {
                return null;
            }

            return combined;
        }

        private static bool IsPathUnderRoot(string rootDir, string candidatePath)
        {
            string rootFull = Path.GetFullPath(rootDir)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            string candidateFull = Path.GetFullPath(candidatePath);
            return candidateFull.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    candidateFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                    rootFull.TrimEnd(Path.DirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath)?.ToLowerInvariant();
            switch (ext)
            {
                case ".pdf":
                    return "application/pdf";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".txt":
                    return "text/plain";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
