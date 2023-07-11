using DotNetCore.CAP;
using NCapIntegration.EventBus.Events;
using NCapIntegration.Interceptors.Attributes;
using NCapIntegration.Services;

namespace NCapIntegration.EventBus
{
    public class StudentCapSubscriber : ICapSubscribe
    {
        private readonly IStudentService _studentService;

        public StudentCapSubscriber(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Uow]
        [CapSubscribe(nameof(StudentCreatedEvent))]
        public async Task OnStudentCreatedEventTrigger(StudentCreatedEvent @event)
        {
            await _studentService.ProcessInsertStudentEventAsync(@event);

            await Task.CompletedTask;
        }

        [Uow]
        [CapSubscribe(nameof(StudentDeletedEvent))]
        public async Task OnStudentDeletedEventTrigger(StudentDeletedEvent @event)
        {
            await _studentService.ProcessDeleteStudentEventAsync(@event);

            await Task.CompletedTask;
        }
    }
}
