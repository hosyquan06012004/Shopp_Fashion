using Microsoft.AspNetCore.Mvc;
using BTL_Mongodb.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq;
using BTL_Mongodb.Models.Data;
using X.PagedList.Extensions;
using BTL_Mongodb.Models.Atributes;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Role", "View")]
    public class RolePermissionController : Controller
    {
        private readonly IMongoCollection<Role> _roles;
        private readonly IMongoCollection<Permission> _permissions;
        private readonly IMongoCollection<RolePermission> _rolePermissions;

        public RolePermissionController(MongoDbContext context)
        {
            _roles = context.Roles;
            _permissions = context.Permissions;
            _rolePermissions = context.RolePermissions;
        }

        // Hiển thị danh sách role và các quyền
      
        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 6; // Số lượng role mỗi trang
            int pageNumber = page ?? 1;

            var rolesQuery = await _roles.Find(_ => true).ToListAsync();

            // Tìm kiếm theo tên role nếu có
            if (!string.IsNullOrEmpty(search))
            {
                rolesQuery = rolesQuery
                    .Where(r => r.Name != null && r.Name.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            var pagedList = rolesQuery.ToPagedList(pageNumber, pageSize);
            ViewBag.Search = search;
            return View(pagedList);
        }


        [AuthorizePermission("Role", "Assign")]
        // Hiển thị form gán quyền cho role
        public async Task<IActionResult> Assign(string id) // id = roleId
        {
            var role = await _roles.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (role == null) return NotFound();

            var allPermissions = await _permissions.Find(_ => true).ToListAsync();
            var assigned = await _rolePermissions.Find(rp => rp.RoleId == id).ToListAsync();
            var assignedIds = assigned.Select(rp => rp.PermissionId).ToList();

            ViewBag.Permissions = allPermissions;
            ViewBag.Assigned = assignedIds;
            ViewBag.Role = role;

            return View();
        }

        // Xử lý lưu quyền
        [HttpPost]
        public async Task<IActionResult> Assign(string roleId, List<string> permissionIds)
        {
            // Xoá hết quyền cũ
            await _rolePermissions.DeleteManyAsync(rp => rp.RoleId == roleId);

            // Thêm quyền mới
            var newPermissions = permissionIds.Select(pId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pId
            }).ToList();

            if (newPermissions.Any())
                await _rolePermissions.InsertManyAsync(newPermissions);

            TempData["success"] = "Gán quyền thành công!";
            return RedirectToAction("Index");
        }


        [AuthorizePermission("Role", "Create")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            if (!ModelState.IsValid)
                return View(role);

            await _roles.InsertOneAsync(role);

            TempData["success"] = "Tạo role mới thành công!";
            return RedirectToAction("Index");
        }


        [AuthorizePermission("Role", "Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roles.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (role == null) return NotFound();

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Role updatedRole)
        {
            if (!ModelState.IsValid)
                return View(updatedRole);

            await _roles.ReplaceOneAsync(r => r.Id == id, updatedRole);
            TempData["success"] = "Cập nhật role thành công!";
            return RedirectToAction("Index");
        }


        //[AuthorizePermission("Role", "Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roles.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (role == null)
            {
                TempData["error"] = "Không tìm thấy role để xoá!";
                return RedirectToAction("Index");
            }

            // Xoá các quyền liên kết
            await _rolePermissions.DeleteManyAsync(rp => rp.RoleId == id);

            // Xoá chính Role đó
            await _roles.DeleteOneAsync(r => r.Id == id);

            TempData["success"] = "Xoá người dùng thành công!";
            return RedirectToAction("Index");
        }



    }
}
