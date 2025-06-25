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
            // Database connection
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<NpgsqlConnection>(_ => new NpgsqlConnection(connectionString));

            // Services
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            // ViewModels
            services.AddTransient<TableManagerViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
        }
    }
}