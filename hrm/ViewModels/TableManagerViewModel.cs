using CommunityToolkit.Mvvm.Input;
using Npgsql;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace hrm.ViewModels
{
    public class TableManagerViewModel : ViewModelBase
    {
        private readonly string _connectionString;
        private DataTable _currentTable;
        private string _currentTableName;
        private DataRowView _selectedRow;

        public DataTable CurrentTable
        {
            get => _currentTable;
            set { _currentTable = value; OnPropertyChanged(); }
        }

        public DataRowView SelectedRow
        {
            get => _selectedRow;
            set { _selectedRow = value; OnPropertyChanged(); }
        }

        public ICommand LoadTableCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public TableManagerViewModel(string connectionString)
        {
            _connectionString = connectionString;

            LoadTableCommand = new RelayCommand<string>(LoadTable);
            RefreshCommand = new RelayCommand(RefreshData);
            AddCommand = new RelayCommand(AddRecord);
            EditCommand = new RelayCommand(EditRecord, () => SelectedRow != null);
            DeleteCommand = new RelayCommand(DeleteRecord, () => SelectedRow != null);
            SaveCommand = new RelayCommand(SaveChanges);
        }

        private void LoadTable(string tableName)
        {
            _currentTableName = tableName;
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var adapter = new NpgsqlDataAdapter($"SELECT * FROM {_currentTableName}", connection);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    CurrentTable = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddRecord()
        {
            var newRow = CurrentTable.NewRow();
            CurrentTable.Rows.Add(newRow);
            SelectedRow = (DataRowView)CurrentTable.DefaultView[CurrentTable.Rows.Count - 1];
        }

        private void EditRecord()
        {
            // Реализация редактирования
            MessageBox.Show("Редактирование записи");
        }

        private void DeleteRecord()
        {
            try
            {
                var primaryKey = GetPrimaryKeyColumn(_currentTableName);
                var id = SelectedRow[primaryKey];

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(
                        $"DELETE FROM {_currentTableName} WHERE {primaryKey} = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }

        private void SaveChanges()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var adapter = new NpgsqlDataAdapter($"SELECT * FROM {_currentTableName}", connection);
                    var builder = new NpgsqlCommandBuilder(adapter);
                    adapter.Update(CurrentTable);
                    MessageBox.Show("Изменения сохранены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private string GetPrimaryKeyColumn(string tableName)
        {
            // Аналогично реализации вашего друга
            return "id"; // Упрощённо, лучше реализовать как в примере друга
        }
    }
}