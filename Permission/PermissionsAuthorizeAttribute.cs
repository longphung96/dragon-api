using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DragonAPI.Permission
{
    public class PermissionsAuthorizeAttribute : AuthorizeAttribute
    {
        public PermissionsAuthorizeAttribute(string policy) : base(policy)
        {
        }
    }
}