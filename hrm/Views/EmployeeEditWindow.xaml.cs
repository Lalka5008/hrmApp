using hrm.Models;
using System.Windows;

namespace hrm.Views
{
    public partial class EmployeeEditWindow : Window
    {
        public EmployeeEditWindow(Employee employee)
        {
            InitializeComponent();
            DataContext = employee;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}