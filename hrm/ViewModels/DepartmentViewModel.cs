using System.Collections.ObjectModel;
using hrm.Models;
using hrm.Services;
namespace hrm.ViewModels
{
    public class DepartmentViewModel : ViewModelBase
    {
        private readonly IDepartmentService _departmentService;
        private ObservableCollection<Department> _departments;

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged();
            }
        }

        public DepartmentViewModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
            LoadDepartments();
        }

        private async void LoadDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments = new ObservableCollection<Department>(departments);
        }
    }
}