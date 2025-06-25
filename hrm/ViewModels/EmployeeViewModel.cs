using System.Collections.ObjectModel;
using hrm.Models;
using hrm.Services;
namespace hrm.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<Employee> _employees;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public EmployeeViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            LoadEmployees();
        }

        private async void LoadEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            Employees = new ObservableCollection<Employee>(employees);
        }
    }
}