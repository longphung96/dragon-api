using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DragonAPI.Services
{
    public class IdentityService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string CurrentUserId => httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string CurrentUsername => httpContextAccessor.HttpContext.User.FindFirstValue("name");
        public string ProfilePicture => httpContextAccessor.HttpContext.User.FindFirstValue("picture");
    }
}