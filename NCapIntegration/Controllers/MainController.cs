using Microsoft.AspNetCore.Mvc;
using NCapIntegration.Services;

namespace NCapIntegration.Controllers
{
    /// <summary>
    /// 测试用控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public MainController(IStudentService studentService)
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

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stus = await _studentService.GetAllStudentsAsync();
            return Ok(stus);
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
