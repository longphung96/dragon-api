using Microsoft.AspNetCore.Authorization;

namespace DragonAPI.Permission
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public string[] Permissions { get; private set; }
        private const char Delimiter = ':';

        public PermissionsRequirement(string[] permissions)
        {
            Permissions = permissions;
        }
        public bool HasPrefix(string candidate)
        {
            foreach (var permission in Permissions)
            {
                var valid = HasPrefix(permission, candidate);
                if (!valid)
                    return false;
            }
            return true;
        }
        private bool HasPrefix(string permission, string candidate)
        {
            int i = 0;
            int permissionLength = permission.Length, pl = candidate.Length;

            // prefix longer than scope
            if (pl > permissionLength) return false;

            while (i < pl && permission[i] == candidate[i]) ++i;

            return i == pl && (pl == permissionLength || permission[i] == Delimiter);
        }
    }
}