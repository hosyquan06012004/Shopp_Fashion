using BTL_Mongodb.Models;
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Banner", "View")]
    public class BannerController : Controller
    {
        private const int DefaultPageSize = 4;
        private readonly IRepositoryBanner _bannerRepo;
        private readonly Upload _uploadService;

        public BannerController(IRepositoryBanner bannerRepo)
        {
            _bannerRepo = bannerRepo;
            _uploadService = new Upload();
        }

        public IActionResult Index(int page = 1, string searchString = "", int pageSize = 0)
        {
            if (pageSize <= 0) pageSize = DefaultPageSize;

            IPagedList<Banner> model;
            if (string.IsNullOrEmpty(searchString))
                model = _bannerRepo.GetPage(page, pageSize);
            else
            {
                model = _bannerRepo.Search(searchString, page, pageSize);
                ViewBag.SearchString = searchString;
            }

            ViewBag.PageSize = pageSize;
            return View(model);
        }

        [AuthorizePermission("Banner", "Create")]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Banner banner, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View(banner);

            if (image != null && image.Length > 0)
            {
                var img = _uploadService.UploadFile(image, "uploads/banners"); // dùng thư mục riêng cho banner
                banner.Image = img;
            }
            else
            {
                banner.Image = "";
            }

            var result = _bannerRepo.Insert(banner);
            TempData[result ? "success" : "error"] = result ? "Thêm banner thành công!" : "Thêm thất bại!";
            return RedirectToAction(nameof(Index));
        }


        [AuthorizePermission("Banner", "Edit")]
        public IActionResult Edit(string id)
        {
            var banner = _bannerRepo.GetById(id);
            return banner == null ? NotFound() : View(banner);
        }

        [HttpPost]
        public IActionResult Edit(Banner banner, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View(banner);

            var oldBanner = _bannerRepo.GetById(banner.Id);
            if (oldBanner == null) return NotFound();

            if (image != null && image.Length > 0)
            {
                var img = _uploadService.UploadFile(image, "uploads/banners");
                banner.Image = img;
            }
            else
            {
                banner.Image = oldBanner.Image; // giữ nguyên ảnh cũ nếu không chọn mới
            }

            var result = _bannerRepo.Update(banner);
            TempData[result ? "success" : "error"] = result ? "Cập nhật thành công!" : "Cập nhật thất bại!";
            return RedirectToAction(nameof(Index));
        }


        [AuthorizePermission("Banner", "Delete")]
        public IActionResult Delete(string id)
        {
            var result = _bannerRepo.Delete(id);
            TempData[result ? "success" : "error"] = result ? "Xóa thành công!" : "Xóa thất bại!";
            return RedirectToAction(nameof(Index));
        }
    }
}
