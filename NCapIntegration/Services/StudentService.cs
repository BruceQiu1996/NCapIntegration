using Microsoft.EntityFrameworkCore;
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

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _demoDbContext.Set<Student>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            if (student == null)
            {
                return;
            }

            student.IsDeleted = true;
            await _demoDbContext.SaveChangesAsync();
            await _eventPublisher.PublishAsync(new StudentDeletedEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Data = new StudentDeletedEvent.StudentDeletedEventBody()
                {
                    Id = student.Id,
                    Name = student.Name
                }
            });
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _demoDbContext.Set<Student>().ToListAsync();
            //如果希望忽略全局的查询过滤器: _demoDbContext.Set<Student>().IgnoreQueryFilters().ToListAsync()
        }

        public async Task InsertStudentAsync()
        {
            var newStudent = new Student()
            {
                Name = "Test",
                Address = "Test",
                Sex = false,
                Birthday = DateTime.Now,
                IsDeleted = false
            };
            await _demoDbContext.Set<Student>().AddAsync(newStudent);
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
            Console.WriteLine($"receive a new StudentCreatedEvent : {@event.Id}--{@event.Data.Id}--{@event.Data.Age}");

            await Task.CompletedTask;
        }

        /// <summary>
        /// 处理删除学生后的分布式事务消息
        /// </summary>
        /// <param name="event">StudentCreatedEvent</param>
        /// <returns></returns>
        public async Task ProcessDeleteStudentEventAsync(StudentDeletedEvent @event)
        {
            //throw new Exception("error");
            Console.WriteLine($"delete a astudent : id:{@event.Data.Id},name:{@event.Data.Name}");

            await Task.CompletedTask;
        }
    }
}
