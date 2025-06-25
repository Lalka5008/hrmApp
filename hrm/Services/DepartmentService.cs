using Dapper;
using hrm.Models;
using hrm.Services;
using Npgsql;

namespace hrm.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly string _connectionString;

        public DepartmentService(NpgsqlConnection connection)
        {
            _connectionString = connection.ConnectionString;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Department>("SELECT * FROM Departments");
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<Department>(
                "SELECT * FROM Departments WHERE department_id = @Id", new { Id = id });
        }

        public async Task AddDepartmentAsync(Department department)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "INSERT INTO Departments (name, location, manager_id) VALUES (@Name, @Location, @ManagerId)";
            await connection.ExecuteAsync(sql, department);
        }

        public async Task UpdateDepartmentAsync(Department department)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"UPDATE Departments 
                       SET name = @Name, location = @Location, manager_id = @ManagerId 
                       WHERE department_id = @DepartmentId";
            await connection.ExecuteAsync(sql, department);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "DELETE FROM Departments WHERE department_id = @Id", new { Id = id });
        }
    }
}