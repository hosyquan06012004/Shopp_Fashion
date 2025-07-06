using BTL_Mongodb.Models;
using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BTL_Mongodb.Models.Atributes;
using X.PagedList;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]

    [AuthorizePermission("Category", "View")]
    public class CategoryController : Controller
    {
        private readonly IRepositoryCategory _categoryRepository;
        private const int DefaultPageSize = 7;

        public CategoryController(IRepositoryCategory categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index(int page = 1, string searchString = "", int pageSize = 0)
        {
            if (pageSize <= 0) pageSize = DefaultPageSize;

            IPagedList<Category> model;

            if (string.IsNullOrEmpty(searchString))
            {
                model = _categoryRepository.GetPage(page, pageSize);
            }
            else
            {
                model = _categoryRepository.Search(searchString, page, pageSize);
                ViewBag.SearchString = searchString;
            }

            ViewBag.PageSize = pageSize;
            return View(model);
        }

        [AuthorizePermission("Category", "Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
         
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var result = _categoryRepository.Insert(category);
            if (result)
            {
                TempData["success"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Thêm danh mục thất bại!";
                return View(category);
            }
        }

        [AuthorizePermission("Category", "Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var result = _categoryRepository.Update(category);
            if (result)
            {
                TempData["success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Cập nhật danh mục thất bại!";
                return View(category);
            }
        }

        [AuthorizePermission("Category", "Delete")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            bool isDeleted = _categoryRepository.Delete(id);
            if (isDeleted)
            {
                TempData["success"] = "Xóa danh mục thành công!";
            }
            else
            {
                TempData["error"] = "Xóa danh mục thất bại!";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}