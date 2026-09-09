using MasterApi.Services;
using MasterEF;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MasterApi.Controllers
{
    [ApiController]
    [Route("api/{controller}/{action}")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Values()
        {
            return new string[] { "values", "values1" };
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            if (student == null)
            {
                return BadRequest("Student data cannot be found");
            }

            try
            {

                await _studentService.CreateStudentAsync(student);

                return StatusCode(201, "Student Created Successfully.");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudent()
        {
            var student = await _studentService.GetAllStudentAsync();
            return Ok(student);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            try
            {
                Student? student = await _studentService.GetStudentAsync(id);
                if (student == null)
                {
                    return NotFound("Student not found");
                }
                return Ok(student);
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);

                return Ok("Student Deleted Successfully");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudent(Student student)
        {
            if (student == null)
            {
                return BadRequest("Student data cannot be found");
            }

            try
            {
                await _studentService.UpdateStudentAsync(student);

                return Ok("Updated Successfully");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }
    }
}
