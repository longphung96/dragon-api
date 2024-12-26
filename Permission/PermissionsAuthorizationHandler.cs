using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DragonAPI.Services;

namespace DragonAPI.Permission
{
    public class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        private readonly UserService userService;

        public PermissionsAuthorizationHandler(UserService userService)
        {
            this.userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
            var userPermissions = await userService.GetPermissions();
            if (userPermissions != null)
            {
                foreach (var permission in userPermissions)
                {
                    var valid = requirement.HasPrefix(permission);
                    if (valid)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}