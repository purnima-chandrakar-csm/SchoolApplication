using Dapper;
using HomeApi.Services;
using HomeEF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HomeApi.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly string _connectionString;

        public TeacherService(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public async Task<int> CreateTeacherAsync(Teacher teacher)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Insert",
                teacher.TeacherName,
                teacher.TeacherEmail,
                teacher.TeacherPhone,
            };

            return await con.ExecuteAsync("sp_Teacher", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteTeacherAsync(int id)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Delete",
                TeacherId = id,
            };


            return await con.ExecuteAsync("sp_Teacher", parameters, commandType: CommandType.StoredProcedure) > 0;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeacherAsync()
        {
            using var con = new SqlConnection(_connectionString);


            return await con.QueryAsync<Teacher>(
                "sp_Teacher",
                new { Action = "SelectAll" },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            try
            {

                using var con = new SqlConnection(_connectionString);


                return await con.QueryFirstOrDefaultAsync<Teacher>(
                    "sp_Teacher",
                    new { Action = "Select", TeacherId = id },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                return new Teacher();
            }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            using var con = new SqlConnection(_connectionString);

            var parameters = new
            {
                Action = "Update",
                teacher.TeacherName,
                teacher.TeacherEmail,
                teacher.TeacherPhone,
                teacher.TeacherId,
            };

            return await con.ExecuteAsync("sp_Teacher", parameters, commandType: CommandType.StoredProcedure) > 0;
        }

       
    }
}


