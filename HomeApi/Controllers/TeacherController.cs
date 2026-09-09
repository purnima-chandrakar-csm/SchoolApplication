using HomeApi.Services;
using HomeEF;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HomeApi.Controllers
{
    [ApiController]
    [Route("api/{controller}/{action}")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Values()
        {
            return new string[] { "values", "values1" };
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data cannot be found");
            }

            try
            {

                await _teacherService.CreateTeacherAsync(teacher);

                return StatusCode(201, "Teacher Created Successfully.");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacher()
        {
            var teacher = await _teacherService.GetAllTeacherAsync();
            return Ok(teacher);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            try
            {
                Teacher? teacher = await _teacherService.GetTeacherAsync(id);
                if (teacher == null)
                {
                    return NotFound("Teacher not found");
                }
                return Ok(teacher);
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            try
            {
                await _teacherService.DeleteTeacherAsync(id);

                return Ok("Teacher Deleted Successfully");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTeacher(Teacher teacher)
        {
            if (teacher == null)
            {
                return BadRequest("Teacher data cannot be found");
            }

            try
            {
                await _teacherService.UpdateTeacherAsync(teacher);

                return Ok("Updated Successfully");
            }
            catch (SqlException ex)
            {
                return BadRequest($"Database Error: {ex.Message}");
            }
        }
    }
}


