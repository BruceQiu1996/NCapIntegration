using NCapIntegration.Entities;
using NCapIntegration.EventBus.Events;
using NCapIntegration.Interceptors.Attributes;

namespace NCapIntegration.Services
{
    public interface IStudentService
    {
        [OperateLog]
        [Uow(Distribute = true)]
        Task InsertStudentAsync();

        [Uow]
        Task ProcessInsertStudentEventAsync(StudentCreatedEvent @event);

        [OperateLog]
        Task<IEnumerable<Student>> GetAllStudentsAsync();
    }
}
