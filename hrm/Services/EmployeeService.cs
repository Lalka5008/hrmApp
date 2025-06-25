using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using hrm.Models;
using Npgsql;

namespace hrm.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(NpgsqlConnection connection)
        {
            _connectionString = connection.ConnectionString;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT e.*, d.name as DepartmentName, p.title as PositionTitle 
                      FROM Employees e
                      LEFT JOIN Departments d ON e.department_id = d.department_id
                      LEFT JOIN Positions p ON e.position_id = p.position_id";

            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT e.*, d.name as DepartmentName, p.title as PositionTitle
                      FROM Employees e
                      LEFT JOIN Departments d ON e.department_id = d.department_id
                      LEFT JOIN Positions p ON e.position_id = p.position_id
                      WHERE e.employee_id = @Id";

            return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"INSERT INTO Employees 
                      (first_name, last_name, birth_date, gender, 
                       department_id, position_id, hire_date, status) 
                      VALUES (@FirstName, @LastName, @BirthDate, @Gender, 
                             @DepartmentId, @PositionId, @HireDate, @Status)";

            await connection.ExecuteAsync(sql, employee);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"UPDATE Employees 
                      SET first_name = @FirstName, last_name = @LastName, 
                          birth_date = @BirthDate, gender = @Gender,
                          department_id = @DepartmentId, position_id = @PositionId,
                          hire_date = @HireDate, status = @Status
                      WHERE employee_id = @EmployeeId";

            await connection.ExecuteAsync(sql, employee);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "DELETE FROM Employees WHERE employee_id = @Id", new { Id = id });
        }
    }
}