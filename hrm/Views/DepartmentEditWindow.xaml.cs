using hrm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hrm.Views
{
    public partial class DepartmentEditWindow : Window
    {
        public DepartmentEditWindow(Department department)
        {
            InitializeComponent();
            DataContext = department;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}