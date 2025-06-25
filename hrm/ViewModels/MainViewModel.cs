using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using hrm.Services;
namespace hrm.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentViewModel;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowDepartmentsCommand { get; }
        public ICommand ShowEmployeesCommand { get; }

        public MainViewModel(IDepartmentService departmentService, IEmployeeService employeeService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;

            ShowDepartmentsCommand = new RelayCommand(ShowDepartments);
            ShowEmployeesCommand = new RelayCommand(ShowEmployees);
        }

        private void ShowDepartments()
        {
            CurrentViewModel = new DepartmentViewModel(_departmentService);
        }

        private void ShowEmployees()
        {
            CurrentViewModel = new EmployeeViewModel(_employeeService);
        }
    }
}