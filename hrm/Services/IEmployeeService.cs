using hrm.Models;


namespace hrm.Services
{
    public interface IEmployeeService
    {
        Task AddEmployeesAsync(IEnumerable<Employee> employees);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
    }
}