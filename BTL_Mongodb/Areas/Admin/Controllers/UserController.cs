using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BTL_Mongodb.Models.ViewModel;
using X.PagedList;
using X.PagedList.Extensions;
using BTL_Mongodb.Models.Atributes;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("User", "View")]
    public class UserController : Controller
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Role> _roles;
        private readonly IMongoCollection<UserRole> _userRoles;

        public UserController(MongoDbContext context)
        {
            _users = context.Users;
            _roles = context.Roles;
            _userRoles = context.UserRoles;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 6;
            int pageNumber = page ?? 1;

            var usersQuery = await _users.Find(_ => true).ToListAsync();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery
                    .Where(u => u.Username != null && u.Username.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            var roles = await _roles.Find(_ => true).ToListAsync();
            var userRoles = await _userRoles.Find(_ => true).ToListAsync();

            var model = usersQuery.Select(u =>
            {
                var roleNames = userRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Select(ur => roles.FirstOrDefault(r => r.Id == ur.RoleId)?.Name)
                    .Where(name => name != null)
                    .ToList();

                return new UserViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = roleNames
                };
            }).ToList();

            var pagedList = model.ToPagedList(pageNumber, pageSize);

            ViewBag.Search = search;
            return View(pagedList);
        }


        [AuthorizePermission("User", "Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return NotFound();

            var roles = await _roles.Find(_ => true).ToListAsync();
            var userRoles = await _userRoles.Find(ur => ur.UserId == id).ToListAsync();
            var selectedRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AllRoles = roles,
                SelectedRoleIds = selectedRoleIds
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Field: {error.Key}");
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"   Error: {subError.ErrorMessage}");
                    }
                }

                model.AllRoles = await _roles.Find(_ => true).ToListAsync();
                return View(model);
            }

            var update = Builders<User>.Update
                .Set(u => u.Username, model.Username)
                .Set(u => u.Email, model.Email);

            await _users.UpdateOneAsync(u => u.Id == model.Id, update);

            await _userRoles.DeleteManyAsync(ur => ur.UserId == model.Id);

            if (model.SelectedRoleIds != null && model.SelectedRoleIds.Any())
            {
                var newUserRoles = model.SelectedRoleIds.Select(roleId => new UserRole
                {
                    UserId = model.Id,
                    RoleId = roleId
                });

                await _userRoles.InsertManyAsync(newUserRoles);
            }
            TempData["success"] = "Cập nhật người dùng thành công!";
            return RedirectToAction("Index");
        }

        [AuthorizePermission("User", "Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            // Xoá các bản ghi liên quan trong bảng UserRole
            var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                TempData["error"] = "Không tìm thấy người dùng để xoá!";
                return RedirectToAction("Index");
            }
            // Xoá người dùng
            await _users.DeleteOneAsync(u => u.Id == id);
            TempData["success"] = "Xoá người dùng thành công!";
            return RedirectToAction("Index");
        }


        [AuthorizePermission("User", "Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roles.Find(_ => true).ToListAsync();
            var model = new EditUserViewModel
            {
                AllRoles = roles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EditUserViewModel model, string Password)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = await _roles.Find(_ => true).ToListAsync();
                return View(model);
            }

            // Kiểm tra trùng username hoặc email nếu muốn
            var existingUser = await _users.Find(u => u.Username == model.Username || u.Email == model.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username hoặc Email đã tồn tại.");
                model.AllRoles = await _roles.Find(_ => true).ToListAsync();
                return View(model);
            }

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = Password // Huyn có thể thay bằng BCrypt nếu muốn mã hoá
            };

            await _users.InsertOneAsync(newUser);

            if (model.SelectedRoleIds != null && model.SelectedRoleIds.Any())
            {
                var userRoles = model.SelectedRoleIds.Select(roleId => new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = roleId
                });

                await _userRoles.InsertManyAsync(userRoles);
            }
            TempData["success"] = "Tạo người dùng mới thành công!";

            return RedirectToAction("Index");
        }

    }
}