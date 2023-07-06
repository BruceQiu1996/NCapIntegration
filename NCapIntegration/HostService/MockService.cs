using NCapIntegration.Services;
using Microsoft.Extensions.Hosting;

namespace NCapIntegration.HostService
{
    public class MockService : IHostedService
    {
        private readonly IStudentService _studentService;

        public MockService(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.Clear();
            Console.WriteLine("MockService is starting-----------------");
            _studentService.InsertStudentAsync();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
