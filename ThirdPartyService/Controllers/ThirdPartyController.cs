using Microsoft.AspNetCore.Mvc;

namespace ThirdPartyService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThirdPartyController : ControllerBase
    {
       
        private readonly ILogger<ThirdPartyController> _logger;

        public ThirdPartyController(ILogger<ThirdPartyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("dummyProcess")]
        public OkResult Get()
        {
            return Ok();
        }
    }
}