using BTL_Mongodb.Models.Data;
using MongoDB.Driver;
using System.Text;
using System.Security.Cryptography;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Role> _roles;
        private readonly IMongoCollection<UserRole> _userRoles;
        private readonly IMongoCollection<Permission> _permissions;
        private readonly IMongoCollection<RolePermission> _rolePermissions;

        public AuthService(MongoDbContext context)
        {
            _users = context.Users;
            _roles = context.Roles;
            _userRoles = context.UserRoles;
            _permissions = context.Permissions;
            _rolePermissions = context.RolePermissions;
        }

        public async Task<bool> Register(string username, string email, string password)
        {
            if (await _users.Find(u => u.Username == username || u.Email == email).AnyAsync())
                return false;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            await _users.InsertOneAsync(user);

            var defaultRole = await _roles.Find(r => r.Name == "user").FirstOrDefaultAsync();
            if (defaultRole == null)
            {
                defaultRole = new Role { Name = "user" };
                await _roles.InsertOneAsync(defaultRole);
            }

            await _userRoles.InsertOneAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id
            });

            return true;
        }

        public async Task<(User user, List<string> roles)> Login(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null || user.PasswordHash != HashPassword(password))
                return (null, null);

            var userRoleList = await _userRoles.Find(ur => ur.UserId == user.Id).ToListAsync();
            var roleIds = userRoleList.Select(ur => ur.RoleId);
            var roles = await _roles.Find(r => roleIds.Contains(r.Id)).ToListAsync();

            return (user, roles.Select(r => r.Name).ToList());
        }

        public async Task<bool> HasPermission(string userId, string module, string action)
        {
            var userRoles = await _userRoles.Find(ur => ur.UserId == userId).ToListAsync();
            var roleIds = userRoles.Select(r => r.RoleId);

            var rolePermissions = await _rolePermissions.Find(rp => roleIds.Contains(rp.RoleId)).ToListAsync();
            var permissionIds = rolePermissions.Select(p => p.PermissionId);

            var permission = await _permissions
                .Find(p => permissionIds.Contains(p.Id) && p.Module == module && p.Action == action)
                .FirstOrDefaultAsync();

            return permission != null;
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetRolesForUserAsync(string userId)
        {
            var userRoleList = await _userRoles.Find(ur => ur.UserId == userId).ToListAsync();
            var roleIds = userRoleList.Select(ur => ur.RoleId);
            var roles = await _roles.Find(r => roleIds.Contains(r.Id)).ToListAsync();

            return roles.Select(r => r.Name).ToList();
        }

    }
}
