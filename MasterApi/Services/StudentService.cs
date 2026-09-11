using MasterEF;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System;

namespace MasterApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly string _connectionString;

        public StudentService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateStudentAsync(Student student)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Insert",
                student.StudentName,
                student.StudentEmail,
                student.StudentPhone,
            };

            return await con.ExecuteAsync("Pcsp_Student", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Delete",
                StudentId = id,
            };

           
            return await con.ExecuteAsync("Pcsp_Student", parameters, commandType: CommandType.StoredProcedure) > 0;
        }

        public async Task<IEnumerable<Student>> GetAllStudentAsync()
        {
            using var con = new SqlConnection(_connectionString);

         
            return await con.QueryAsync<Student>(
                "Pcsp_Student",
                new { Action = "SelectAll" },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            try
            {

                using var con = new SqlConnection(_connectionString);


                return await con.QueryFirstOrDefaultAsync<Student>(
                    "Pcsp_Student",
                    new { Action = "Select", StudentId = id },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch(Exception ex)
            {
                return new Student();
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Update",
                student.StudentName,
                student.StudentEmail,
                student.StudentPhone,
                student.StudentId,
            };

            return await con.ExecuteAsync("Pcsp_Student", parameters, commandType: CommandType.StoredProcedure) > 0;
        }
    }
}
