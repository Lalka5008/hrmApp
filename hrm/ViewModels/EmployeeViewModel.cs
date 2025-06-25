using CommunityToolkit.Mvvm.Input;
using hrm.Models;
using hrm.Services;
using hrm.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace hrm.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private ObservableCollection<Employee> _employees;
        private Employee _selectedEmployee;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set { _employees = value; OnPropertyChanged(); }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public EmployeeViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;

            AddCommand = new RelayCommand(AddEmployee);
            EditCommand = new RelayCommand(EditEmployee, CanEditDelete);
            DeleteCommand = new RelayCommand(DeleteEmployee, CanEditDelete);
            RefreshCommand = new RelayCommand(LoadEmployees);

            LoadEmployees();
        }

        private bool CanEditDelete() => SelectedEmployee != null;

        private async void LoadEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                Employees = new ObservableCollection<Employee>(employees);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private async void AddEmployee()
        {
            var newEmployee = new Employee
            {
                HireDate = DateTime.Today,
                Status = "Active",
                BirthDate = DateTime.Now.AddYears(-20) // Установим разумное значение по умолчанию
            };

            if (ShowEditDialog(newEmployee))
            {
                try
                {
                    await _employeeService.AddEmployeeAsync(newEmployee);
                    LoadEmployees();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}");
                }
            }
        }

        private void EditEmployee()
        {
            if (SelectedEmployee == null) return;

            var employeeCopy = new Employee
            {
                EmployeeId = SelectedEmployee.EmployeeId,
                FirstName = SelectedEmployee.FirstName,
                LastName = SelectedEmployee.LastName,
                BirthDate = SelectedEmployee.BirthDate,
                Gender = SelectedEmployee.Gender,
                DepartmentId = SelectedEmployee.DepartmentId,
                PositionId = SelectedEmployee.PositionId,
                HireDate = SelectedEmployee.HireDate,
                Status = SelectedEmployee.Status
            };

            if (ShowEditDialog(employeeCopy))
            {
                _employeeService.UpdateEmployeeAsync(employeeCopy);
                LoadEmployees();
            }
        }

        private async void DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            if (MessageBox.Show("Удалить этого сотрудника?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeId);
                LoadEmployees();
            }
        }

        private bool ShowEditDialog(Employee employee)
        {
            var dialog = new EmployeeEditWindow(employee);

            // Установим владельца для правильного позиционирования
            dialog.Owner = Application.Current.MainWindow;

            return dialog.ShowDialog() == true;
        }
    }
}