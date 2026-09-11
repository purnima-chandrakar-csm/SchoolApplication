using FileStorage;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MasterApi.Controllers
{
    [ApiController]
    [Route("api/{controller}/{action}")]
    public class FileController : ControllerBase
    {
        private readonly ILocalFileService _localFileService;

        public FileController(ILocalFileService localFileService)
        {
            _localFileService = localFileService;
        }

        [HttpPost]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> SaveFileToLocaBase64([FromBody] MyFileRequest myFileRequest)
        {
            if (myFileRequest == null)
            {
                return BadRequest("File data cannot be found");
            }

            MessageEF msg = await _localFileService.SaveFileToLocaBase64(myFileRequest);
            if (msg.Satus == "True")
            {
                return Ok(msg);
            }

            return BadRequest(msg);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFileFromLocal([FromQuery] string path)
        {
            MyFileResult result = await _localFileService.DownloadFileFromLocal(path);
            if (result?.Filestream == null)
            {
                return NotFound("File not found");
            }

            return File(result.Filestream, result.ContentType, result.FileName);
        }
    }
}
