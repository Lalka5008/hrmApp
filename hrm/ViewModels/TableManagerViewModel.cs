using CommunityToolkit.Mvvm.Input;
using Dapper;
using hrm.Models;
using hrm.Services;
using Microsoft.Win32;
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
        private readonly IJsonService _jsonService;
        private DataRowView _selectedRow;
        private bool CanExportImport() => _currentTableName?.ToLower() == "employees";


        public DataTable CurrentTable
        {
            get => _currentTable;
            set { _currentTable = value; OnPropertyChanged(); }
        }

        public DataRowView SelectedRow
        {
            get => _selectedRow;
            set
            {
                _selectedRow = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRowSelected));
                // Обновляем состояние команд
                ((RelayCommand)EditCommand).NotifyCanExecuteChanged();
                ((RelayCommand)DeleteCommand).NotifyCanExecuteChanged();
            }
        }

        public bool IsRowSelected => SelectedRow != null;

        public ICommand LoadTableCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExportToJsonCommand { get; }
        public ICommand ImportFromJsonCommand { get; }

        public TableManagerViewModel(string connectionString, IJsonService jsonService)
        {
            _connectionString = connectionString;
            _jsonService = jsonService;

            LoadTableCommand = new RelayCommand<string>(LoadTable);
            RefreshCommand = new RelayCommand(RefreshData);
            AddCommand = new RelayCommand(AddRecord);
            EditCommand = new RelayCommand(EditRecord, () => IsRowSelected);
            DeleteCommand = new RelayCommand(DeleteRecord, () => IsRowSelected);
            SaveCommand = new RelayCommand(SaveChanges);
            ExportToJsonCommand = new RelayCommand(ExportToJson, CanExportImport);
            ImportFromJsonCommand = new RelayCommand(ImportFromJson, CanExportImport);
            ExportToJsonCommand = new RelayCommand(ExportToJson, () => CurrentTable != null && _currentTableName?.ToLower() == "employees");
            ImportFromJsonCommand = new RelayCommand(ImportFromJson, () => _currentTableName?.ToLower() == "employees");
            // Обновляем состояние команд при изменении таблицы
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CurrentTable))
                {
                    ((RelayCommand)ExportToJsonCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)ImportFromJsonCommand).NotifyCanExecuteChanged();
                }
            };

        }
        private async void ExportToJson()
        {
            if (CurrentTable == null) return;

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    FileName = $"{_currentTableName}_export.json"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var employees = ConvertDataTableToEmployees(CurrentTable);
                    await _jsonService.ExportEmployeesToJsonAsync(employees, saveFileDialog.FileName);
                    MessageBox.Show("Экспорт завершен успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }

        private async void ImportFromJson()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var employees = await _jsonService.ImportEmployeesFromJsonAsync(openFileDialog.FileName);

                    // Здесь нужно добавить логику импорта в БД
                    using (var connection = new NpgsqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        foreach (var employee in employees)
                        {
                            await connection.ExecuteAsync(
                                @"INSERT INTO employees 
                            (first_name, last_name, birth_date, gender, department_id, position_id, hire_date, status) 
                            VALUES (@FirstName, @LastName, @BirthDate, @Gender, @DepartmentId, @PositionId, @HireDate, @Status)",
                                employee);
                        }
                    }

                    RefreshData();
                    MessageBox.Show($"Импортировано {employees.Count()} записей");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}");
            }
        }

        private IEnumerable<Employee> ConvertDataTableToEmployees(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return new Employee
                {
                    EmployeeId = row.Field<int>("employee_id"),
                    FirstName = row.Field<string>("first_name"),
                    LastName = row.Field<string>("last_name"),
                    BirthDate = row.Field<DateTime>("birth_date"),
                    Gender = row.Field<string>("gender"),
                    DepartmentId = row.Field<int?>("department_id"),
                    PositionId = row.Field<int?>("position_id"),
                    HireDate = row.Field<DateTime>("hire_date"),
                    Status = row.Field<string>("status")
                };
            }
        }
        private void LoadTable(string tableName)
        {
            _currentTableName = tableName;
            RefreshData();
            LogTableStructure(); // Добавьте эту строку для отладки
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
            try
            {
                var newRow = CurrentTable.NewRow();
                CurrentTable.Rows.Add(newRow);
                SelectedRow = (DataRowView)CurrentTable.DefaultView[CurrentTable.Rows.Count - 1];
            }
            catch 
            {
                MessageBox.Show($"Выберите таблицу");
            }
        }

        private void EditRecord()
        {
            if (SelectedRow == null) return;
            MessageBox.Show($"Редактирование записи с ID: {GetIdValue()}");
        }

        private void DeleteRecord()
        {
            if (SelectedRow == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            try
            {
                // Получаем первичный ключ для текущей таблицы
                var primaryKey = GetPrimaryKeyColumn(_currentTableName);
                if (string.IsNullOrEmpty(primaryKey))
                {
                    MessageBox.Show("Не удалось определить первичный ключ таблицы");
                    return;
                }

                // Получаем значение первичного ключа
                var primaryKeyValue = SelectedRow[primaryKey];
                if (primaryKeyValue == null || primaryKeyValue == DBNull.Value)
                {
                    MessageBox.Show("Не удалось получить значение первичного ключа");
                    return;
                }

                // Запрос подтверждения
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить эту запись? (PK: {primaryKey}={primaryKeyValue})",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                // Выполняем удаление
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand(
                        $"DELETE FROM {_currentTableName} WHERE {primaryKey} = @pKey", connection))
                    {
                        cmd.Parameters.AddWithValue("pKey", primaryKeyValue);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно удалена");
                            RefreshData();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить запись. Возможно, она уже была удалена.");
                        }
                    }
                }
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            {
                MessageBox.Show("Нельзя удалить запись, так как на неё есть ссылки в других таблицах");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n\nДетали: {ex.InnerException?.Message}");
            }
        }
        private void LogTableStructure()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new NpgsqlCommand($@"
                SELECT column_name, data_type 
                FROM information_schema.columns 
                WHERE table_name = @tableName", connection);

                    cmd.Parameters.AddWithValue("tableName", _currentTableName.ToLower());

                    using (var reader = cmd.ExecuteReader())
                    {
                        var columns = new System.Text.StringBuilder();
                        columns.AppendLine($"Структура таблицы {_currentTableName}:");

                        while (reader.Read())
                        {
                            columns.AppendLine($"{reader.GetString(0)} ({reader.GetString(1)})");
                        }

                        Console.WriteLine(columns.ToString());
                        // Для отладки можно вывести в MessageBox:
                        // MessageBox.Show(columns.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении структуры таблицы: {ex.Message}");
            }
        }
        private object GetIdValue()
        {
            try
            {
                return SelectedRow["id"] ?? SelectedRow[0];
            }
            catch
            {
                return null;
            }
        }
        private string GetPrimaryKeyColumn(string tableName)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new NpgsqlCommand(@"
                SELECT a.attname
                FROM pg_index i
                JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
                WHERE i.indrelid = @tableName::regclass
                AND i.indisprimary", connection);

                    cmd.Parameters.AddWithValue("tableName", tableName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка определения первичного ключа: {ex.Message}");
                return null;
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

    }
}