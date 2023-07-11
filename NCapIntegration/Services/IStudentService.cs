using NCapIntegration.Entities;
using NCapIntegration.EventBus.Events;
using NCapIntegration.Interceptors.Attributes;

namespace NCapIntegration.Services
{
    public interface IStudentService
    {
        /// <summary>
        /// 新建一个学生
        /// </summary>
        /// <returns>异步task</returns>
        [OperateLog]
        [Uow(Distribute = true)]
        Task InsertStudentAsync();

        /// <summary>
        /// 删除一个学生
        /// </summary>
        /// <returns>异步task</returns>
        [OperateLog]
        [Uow(Distribute = true)]
        Task DeleteStudentAsync(int id);

        /// <summary>
        /// 获取所有学生
        /// </summary>
        /// <returns>所有的学生，默认会走全局过滤器，过滤掉delete的学生</returns>
        [OperateLog]
        Task<IEnumerable<Student>> GetAllStudentsAsync();

        #region cap事件处理
        //cap事件处理

        [Uow]
        Task ProcessInsertStudentEventAsync(StudentCreatedEvent @event);


        [Uow]
        Task ProcessDeleteStudentEventAsync(StudentDeletedEvent @event);
        #endregion
    }
}
