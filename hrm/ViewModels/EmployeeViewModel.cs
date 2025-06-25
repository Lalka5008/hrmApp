// EmployeeViewModel.cs
using CommunityToolkit.Mvvm.Input;
using hrm.Models;
using hrm.Services;
using hrm.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace hrm.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IJsonService _jsonService;
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
        public ICommand ExportToJsonCommand { get; }
        public ICommand ImportFromJsonCommand { get; }

        public EmployeeViewModel(IEmployeeService employeeService, IJsonService jsonService)
        {
            _employeeService = employeeService;
            _jsonService = jsonService;

            AddCommand = new RelayCommand(AddEmployee);
            EditCommand = new RelayCommand(EditEmployee, CanEditDelete);
            DeleteCommand = new RelayCommand(DeleteEmployee, CanEditDelete);
            RefreshCommand = new RelayCommand(LoadEmployees);
            ExportToJsonCommand = new RelayCommand(ExportToJson);
            ImportFromJsonCommand = new RelayCommand(ImportFromJson);

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
        private async void ExportToJson()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json",
                    FileName = "employees_export.json"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    await _jsonService.ExportEmployeesToJsonAsync(Employees, saveFileDialog.FileName);
                    MessageBox.Show("Данные успешно экспортированы в JSON", "Экспорт завершен",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в JSON: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ImportFromJson()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var importedEmployees = await _jsonService.ImportEmployeesFromJsonAsync(openFileDialog.FileName);

                    if (MessageBox.Show($"Найдено {importedEmployees.Count()} сотрудников. Импортировать?",
                                       "Подтверждение импорта",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        foreach (var employee in importedEmployees)
                        {
                            await _employeeService.AddEmployeeAsync(employee);
                        }

                        LoadEmployees();
                        MessageBox.Show("Данные успешно импортированы", "Импорт завершен",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте из JSON: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}