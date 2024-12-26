using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DragonAPI.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseController<T> : ControllerBase
    {
        protected readonly ILogger<T> logger;
        public BaseController(ILogger<T> logger)
        {
            this.logger = logger;
        }
    }
}
