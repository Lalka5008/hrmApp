// JsonService.cs
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using hrm.Models;

namespace hrm.Services
{
    public interface IJsonService
    {
        Task ExportEmployeesToJsonAsync(IEnumerable<Employee> employees, string filePath);
        Task<IEnumerable<Employee>> ImportEmployeesFromJsonAsync(string filePath);
    }

    public class JsonService : IJsonService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public async Task ExportEmployeesToJsonAsync(IEnumerable<Employee> employees, string filePath)
        {
            var json = JsonSerializer.Serialize(employees, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<IEnumerable<Employee>> ImportEmployeesFromJsonAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<IEnumerable<Employee>>(json, _jsonOptions);
        }
    }
}