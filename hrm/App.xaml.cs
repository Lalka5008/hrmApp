using hrm.Services;
using hrm.ViewModels;
using hrm.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Windows;


namespace hrm
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            await TestDatabaseConnection();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
        private async Task TestDatabaseConnection()
        {
            try
            {
                using var connection = new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                MessageBox.Show("Database connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Регистрация сервисов
            services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));
            services.AddScoped<IJsonService, JsonService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            // Регистрация ViewModel с явным указанием параметров
            services.AddTransient<TableManagerViewModel>(provider =>
                new TableManagerViewModel(
                    connectionString,
                    provider.GetRequiredService<IJsonService>()));

            services.AddSingleton<MainWindow>();
        }
    }
}