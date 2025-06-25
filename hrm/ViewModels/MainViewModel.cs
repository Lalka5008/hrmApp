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
        private readonly IJsonService _jsonService;

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

        public MainViewModel(
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            IJsonService jsonService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
            _jsonService = jsonService;

            ShowDepartmentsCommand = new RelayCommand(ShowDepartments);
            ShowEmployeesCommand = new RelayCommand(ShowEmployees);
        }

        private void ShowDepartments()
        {
            CurrentViewModel = new DepartmentViewModel(_departmentService);
        }

        private void ShowEmployees()
        {
            CurrentViewModel = new EmployeeViewModel(_employeeService, _jsonService);
        }
    }
}