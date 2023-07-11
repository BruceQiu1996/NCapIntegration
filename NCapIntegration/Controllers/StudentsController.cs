using Microsoft.AspNetCore.Mvc;
using NCapIntegration.Services;

namespace NCapIntegration.Controllers
{
    /// <summary>
    /// 测试用控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// 创建一个学生对象
        /// </summary>
        /// <returns>执行结果</returns>
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _studentService.InsertStudentAsync();
            return Ok();
        }

        /// <summary>
        /// 查询所有学生
        /// </summary>
        /// <returns>执行结果</returns>
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stus = await _studentService.GetAllStudentsAsync();
            return Ok(stus);
        }

        /// <summary>
        /// 查询所有学生
        /// </summary>
        /// <returns>执行结果</returns>
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return Ok();
        }

        [ApiExplorerSettings(GroupName = "v2")]
        [HttpPost]
        [Route("v2")]
        public async Task<IActionResult> Post_2()
        {
            await _studentService.InsertStudentAsync();
            return Ok();
        }
    }
}
