using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using hrm.Services;
using hrm.ViewModels;
using Npgsql;


namespace hrm
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database connection
            services.AddScoped(_ => new NpgsqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            // Services
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            // Add other services as needed

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<DepartmentViewModel>();
            services.AddTransient<EmployeeViewModel>();
            // Add other ViewModels

            // Views
            services.AddSingleton<MainWindow>();
        }
    }
}