using Dapper;
using hrm.Models;
using hrm.Services;
using Npgsql;

namespace HRManagementApp.Services
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

            var sql = @"SELECT e.*, d.*, p.* 
                       FROM Employees e
                       LEFT JOIN Departments d ON e.department_id = d.department_id
                       LEFT JOIN Positions p ON e.position_id = p.position_id";

            return await connection.QueryAsync<Employee, Department, Position, Employee>(
                sql,
                (employee, department, position) =>
                {
                    employee.Department = department;
                    employee.Position = position;
                    return employee;
                },
                splitOn: "department_id,position_id");
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT e.*, d.*, p.* 
                       FROM Employees e
                       LEFT JOIN Departments d ON e.department_id = d.department_id
                       LEFT JOIN Positions p ON e.position_id = p.position_id
                       WHERE e.employee_id = @Id";

            var result = await connection.QueryAsync<Employee, Department, Position, Employee>(
                sql,
                (employee, department, position) =>
                {
                    employee.Department = department;
                    employee.Position = position;
                    return employee;
                },
                new { Id = id },
                splitOn: "department_id,position_id");

            return result.FirstOrDefault();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"INSERT INTO Employees 
                       (first_name, last_name, birth_date, gender, department_id, position_id, hire_date, status) 
                       VALUES (@FirstName, @LastName, @BirthDate, @Gender, @DepartmentId, @PositionId, @HireDate, @Status)";

            await connection.ExecuteAsync(sql, employee);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"UPDATE Employees 
                       SET first_name = @FirstName, last_name = @LastName, birth_date = @BirthDate, 
                           gender = @Gender, department_id = @DepartmentId, position_id = @PositionId, 
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