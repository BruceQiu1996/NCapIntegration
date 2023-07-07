using NCapIntegration.Entities;
using NCapIntegration.EventBus;
using NCapIntegration.EventBus.Events;
using NCapIntegration.Persistence.MSSql;

namespace NCapIntegration.Services
{
    public class StudentService : IStudentService
    {
        private readonly NCapIntegrationDbContext _demoDbContext;
        private readonly IEventPublisher _eventPublisher;

        public StudentService(NCapIntegrationDbContext demoDbContext, IEventPublisher eventPublisher)
        {
            _demoDbContext = demoDbContext;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 新建一个学生
        /// </summary>
        /// <returns></returns>
        public async Task InsertStudentAsync()
        {
            var newStudent = new Student()
            {
                Name = "Test",
                Address = "Test",
                Sex = false,
                Birthday = DateTime.Now,
            };
            await _demoDbContext.Students.AddAsync(newStudent);

            await _demoDbContext.SaveChangesAsync();
            await _eventPublisher.PublishAsync(new StudentCreatedEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new StudentCreatedEvent.StudentCreatedEventBody()
                {
                    Id = newStudent.Id,
                    Name = newStudent.Name,
                    Age = (int)(DateTime.Now - newStudent.Birthday).TotalMicroseconds, //假设的一个年龄计算方式
                }
            });
        }

        /// <summary>
        /// 处理新建学生后的分布式事务消息
        /// </summary>
        /// <param name="event">StudentCreatedEvent</param>
        /// <returns></returns>
        public async Task ProcessInsertStudentEventAsync(StudentCreatedEvent @event)
        {
            //throw new Exception("error");
            Console.WriteLine($"{@event.Id}--{@event.Data.Id}--{@event.Data.Age}");

            await Task.CompletedTask;
        }
    }
}
