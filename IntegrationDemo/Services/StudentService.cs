using IntegrationDemo.Interceptors.Attributes;
using IntegrationDemo.Persistence;

namespace IntegrationDemo.Services
{
    public class StudentService
    {
        private readonly DemoDbContext _demoDbContext;
        public StudentService(DemoDbContext demoDbContext)
        {
            _demoDbContext = demoDbContext;
        }

        [Uow]
        public async Task InserStudentAsync()
        {
            await _demoDbContext.Students.AddAsync(new Entities.Student()
            {
                Name = "Test",
                Address = "Test",
                Sex = false,
                Birthday = DateTime.Now,
            });
        }
    }
}
