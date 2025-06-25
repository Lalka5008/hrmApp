using CommunityToolkit.Mvvm.Input;
using hrm.Models;
using hrm.Services;
using hrm.ViewModels;
using hrm.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
namespace hrm.ViewModels
{
    public class DepartmentViewModel : ViewModelBase
    {
        private readonly IDepartmentService _departmentService;
        private ObservableCollection<Department> _departments;
        private Department _selectedDepartment;

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set { _departments = value; OnPropertyChanged(); }
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set { _selectedDepartment = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public DepartmentViewModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;

            AddCommand = new RelayCommand(AddDepartment);
            EditCommand = new RelayCommand(EditDepartment, CanEditDelete);
            DeleteCommand = new RelayCommand(DeleteDepartment, CanEditDelete);
            RefreshCommand = new RelayCommand(LoadDepartments);

            LoadDepartments();
        }

        private bool CanEditDelete() => SelectedDepartment != null;

        private async void LoadDepartments()
        {
            try
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                Departments = new ObservableCollection<Department>(departments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отделов: {ex.Message}");
            }
        }

        private void AddDepartment()
        {
            var newDepartment = new Department();
            if (ShowEditDialog(newDepartment))
            {
                _departmentService.AddDepartmentAsync(newDepartment);
                LoadDepartments();
            }
        }

        private void EditDepartment()
        {
            if (SelectedDepartment == null) return;

            var departmentCopy = new Department
            {
                DepartmentId = SelectedDepartment.DepartmentId,
                Name = SelectedDepartment.Name,
                Location = SelectedDepartment.Location,
                ManagerId = SelectedDepartment.ManagerId
            };

            if (ShowEditDialog(departmentCopy))
            {
                _departmentService.UpdateDepartmentAsync(departmentCopy);
                LoadDepartments();
            }
        }

        private async void DeleteDepartment()
        {
            if (SelectedDepartment == null) return;

            if (MessageBox.Show("Удалить этот отдел?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _departmentService.DeleteDepartmentAsync(SelectedDepartment.DepartmentId);
                LoadDepartments();
            }
        }

        private bool ShowEditDialog(Department department)
        {
            var dialog = new DepartmentEditWindow(department);
            return dialog.ShowDialog() == true;
        }
    }
}