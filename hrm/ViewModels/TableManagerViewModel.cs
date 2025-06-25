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

        public TableManagerViewModel(string connectionString)
        {
            _connectionString = connectionString;

            LoadTableCommand = new RelayCommand<string>(LoadTable);
            RefreshCommand = new RelayCommand(RefreshData);
            AddCommand = new RelayCommand(AddRecord);
            EditCommand = new RelayCommand(EditRecord, () => IsRowSelected);
            DeleteCommand = new RelayCommand(DeleteRecord, () => IsRowSelected);
            SaveCommand = new RelayCommand(SaveChanges);
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
            var newRow = CurrentTable.NewRow();
            CurrentTable.Rows.Add(newRow);
            SelectedRow = (DataRowView)CurrentTable.DefaultView[CurrentTable.Rows.Count - 1];
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