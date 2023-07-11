using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace NCapIntegration.Controllers
{
    /// <summary>
    /// ip限流控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IpRateLimitAccessController : ControllerBase
    { 
        private readonly IpRateLimitOptions _options;
        private readonly IIpPolicyStore _ipPolicyStore;

        /// <summary>
        /// ip限流控制器的构造器
        /// </summary>
        /// <param name="optionsAccessor"></param>
        /// <param name="ipPolicyStore"></param>
        public IpRateLimitAccessController(IOptions<IpRateLimitOptions> optionsAccessor, IIpPolicyStore ipPolicyStore)
        {
            _options = optionsAccessor.Value;
            _ipPolicyStore = ipPolicyStore;
        }

        /// <summary>
        /// 获取限制规则
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        { 
            return Ok(await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix));
        }

        /// <summary>
        /// 增加规则
        /// 可以在某些服务被爬虫或者受到大流量访问的时候，通过rpc或者eventbus调用接口进行动态限制
        /// </summary>
        [HttpPost]
        public async Task Post(IpRateLimitPolicy ipRate)
        {
            var pol = await _ipPolicyStore.GetAsync(_options.IpPolicyPrefix);
            if (ipRate != null)
            {
                pol.IpRules.Add(ipRate);
                await _ipPolicyStore.SetAsync(_options.IpPolicyPrefix, pol);
            }
        }
    }
}
