using IntegrationDemo.Services;
using Microsoft.Extensions.Hosting;

namespace IntegrationDemo.HostService
{
    public class MockService : IHostedService
    {
        private readonly StudentService _studentService;

        public MockService(StudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.Clear();
            Console.WriteLine("MockService is starting-----------------");
            await _studentService.InserStudentAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
