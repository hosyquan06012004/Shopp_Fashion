using BTL_Mongodb.Models;
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using X.PagedList;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Brand", "View")]
    public class BrandController : Controller
    {
        private const int DefaultPageSize = 4;
        private readonly IRepositoryBrand _brandRepository;
        private readonly Upload _uploadService;

        public BrandController(IRepositoryBrand brandRepository)
        {
            _brandRepository = brandRepository;
            _uploadService = new Upload();
        }

        public IActionResult Index(int page = 1, string searchString = "", int pageSize = 0)
        {
            if (pageSize <= 0) pageSize = DefaultPageSize;
            IPagedList<Brand> model;
            if (string.IsNullOrEmpty(searchString))
            {
                model = _brandRepository.GetPage(page, pageSize);
            }else
            {
                model = _brandRepository.Search(searchString, page, pageSize);
                ViewBag.SearchString = searchString;
            }
            ViewBag.PageSize = pageSize;
            return View(model);
        }


        [AuthorizePermission("Brand", "Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand, IFormFile? image)
        {
            if(!ModelState.IsValid)
            {
                return View(brand);
            }
            if (image != null && image.Length > 0)
            {
                var img = _uploadService.UploadFile(image, "uploads/students");
                brand.Logo = img;
            }
            else
            {
                brand.Logo = "";
            }
            var result = _brandRepository.Insert(brand);
           
            if (result)
            {
                TempData["success"] = "Thêm thương hiệu thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Thêm thương hiệu thất bại!";
                return View(brand);
            }
         
        }

        [AuthorizePermission("Brand", "Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var brand = _brandRepository.GetById(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            // Lấy lại dữ liệu cũ từ DB
            var oldBrand = _brandRepository.GetById(brand.Id);
            if (oldBrand == null)
            {
                return NotFound();
            }

            // Nếu có ảnh mới, upload ảnh mới và cập nhật
            if (image != null && image.Length > 0)
            {
                var img = _uploadService.UploadFile(image, "uploads/students");
                brand.Logo = img;  // Gán ảnh mới
            }
            else
            {
                brand.Logo = oldBrand.Logo; // Không upload thì giữ nguyên ảnh cũ
            }

            // Cập nhật vào database
            var result = _brandRepository.Update(brand);
            if (result)
            {
                TempData["success"] = "Cập nhật thương hiệu thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Cập nhật thương hiệu thất bại!";
                return View(brand);
            }
        }


        [AuthorizePermission("Brand", "Delete")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var result = _brandRepository.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            bool isDeleted = _brandRepository.Delete(id);
            if (isDeleted)
            {
                TempData["success"] = "Xóa thương hiệu thành công!";
            }
            else
            {
                TempData["error"] = "Xóa thương hiệu thất bại!";
            }
            return RedirectToAction(nameof(Index));
        }
      

        

    }
}
