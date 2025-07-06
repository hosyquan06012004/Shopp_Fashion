using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using X.PagedList.Extensions;
using BTL_Mongodb.Models.Atributes;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Permission", "View")]
    public class PermissionController : Controller
    {
        private readonly IMongoCollection<Permission> _permissions;

        public PermissionController(MongoDbContext context)
        {
            _permissions = context.Permissions;
        }

        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 6; // Số lượng permission mỗi trang
            int pageNumber = page ?? 1;

            var permissionsQuery = await _permissions.Find(_ => true).ToListAsync();

            // Tìm kiếm theo module hoặc action
            if (!string.IsNullOrEmpty(search))
            {
                permissionsQuery = permissionsQuery
                    .Where(p =>
                        (p.Module != null && p.Module.ToLower().Contains(search.ToLower())) ||
                        (p.Action != null && p.Action.ToLower().Contains(search.ToLower())))
                    .ToList();
            }

            var pagedList = permissionsQuery.ToPagedList(pageNumber, pageSize);
            ViewBag.Search = search;
            return View(pagedList);
        }

        [AuthorizePermission("Permission", "Create")]
        // GET: /Admin/Permission/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Permission/Create
        [HttpPost]
        public async Task<IActionResult> Create(Permission permission)
        {
            if (!ModelState.IsValid)
                return View(permission);

            await _permissions.InsertOneAsync(permission);
            TempData["success"] = "Thêm quyền truy cập thành công!";
            return RedirectToAction(nameof(Index));
        }


        [AuthorizePermission("Permission", "Edit")]
        // GET: /Admin/Permission/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var permission = await _permissions.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (permission == null)
            {
                TempData["error"] = "Không tìm thấy quyền cần chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            return View(permission);
        }

        // POST: /Admin/Permission/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Permission updatedPermission)
        {
            if (!ModelState.IsValid)
                return View(updatedPermission);

            var result = await _permissions.ReplaceOneAsync(p => p.Id == id, updatedPermission);
            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                TempData["success"] = "Cập nhật quyền truy cập thành công!";
            }
            else
            {
                TempData["error"] = "Cập nhật quyền thất bại hoặc không có gì thay đổi.";
            }

            return RedirectToAction(nameof(Index));
        }


        [AuthorizePermission("Permission", "Delete")]
        // GET: /Admin/Permission/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            var permission = await _permissions.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (permission == null)
            {
                TempData["error"] = "Không tìm thấy quyền cần xóa.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _permissions.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount > 0)
            {
                TempData["success"] = "Xóa quyền truy cập thành công!";
            }
            else
            {
                TempData["error"] = "Xóa quyền truy cập thất bại!";
            }

            return RedirectToAction(nameof(Index));
        }




    }
}
