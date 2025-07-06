using BTL_Mongodb.Models;
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Product", "View")]
    public class ProductController : Controller
    {
        private const int DefaultPageSize = 5;
        private readonly IRepositoryProduct _productRepository;
        private readonly Upload _uploadService;
        private readonly IRepositoryBrand _brandRepository;
        private readonly IRepositoryCategory _categoryRepository;

        public ProductController(IRepositoryProduct productRepository, IRepositoryBrand brandRepository, IRepositoryCategory categoryRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _uploadService = new Upload();
        }
      
        public IActionResult Index(ProductSearchModel searchModel, int page = 1, int pageSize = 0)
        {
            if (pageSize <= 0) pageSize = DefaultPageSize;
            IPagedList<ProductViewModel> model;

            bool hasSearch = !string.IsNullOrEmpty(searchModel.Name) ||
                             !string.IsNullOrEmpty(searchModel.BrandName) ||
                             !string.IsNullOrEmpty(searchModel.CategoryName) ||
                             searchModel.Price.HasValue;

            if (hasSearch)
            {
                model = _productRepository.Search(searchModel, page, pageSize);
            }
            else
            {
                model = _productRepository.GetFull(page, pageSize);
            }

            ViewBag.PageSize = pageSize;
            return View(model);
        }

        [AuthorizePermission("Product", "Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Brands = _brandRepository.GetAll()
                .Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = b.Id,     // Hoặc b.Id.ToString() nếu cần
                    Text = b.Name
                }).ToList();

            ViewBag.Categories = _categoryRepository.GetAll()
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();

            return View();
        }


        [HttpPost]
        public IActionResult Create(Product product, IFormFile? mainImage, List<IFormFile>? additionalImages)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = _brandRepository.GetAll()
                    .Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = b.Id,     // Hoặc b.Id.ToString() nếu cần
                        Text = b.Name
                    }).ToList();

                ViewBag.Categories = _categoryRepository.GetAll()
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    }).ToList();

                return View(product);
            }

            // Upload ảnh chính
            if (mainImage != null && mainImage.Length > 0)
            {
                var img = _uploadService.UploadFile(mainImage, "uploads/products");
                product.Image = img;
            }
            else
            {
                product.Image = "";
            }

            // Upload nhiều ảnh phụ
            if (additionalImages != null && additionalImages.Any())
            {
                product.Images = new List<string>();
                foreach (var file in additionalImages)
                {
                    if (file != null && file.Length > 0)
                    {
                        var imgPath = _uploadService.UploadFile(file, "uploads/products");
                        product.Images.Add(imgPath);
                    }
                }
            }
            else
            {
                product.Images = new List<string>();
            }



            _productRepository.Insert(product);
            TempData["success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }




        //[HttpPost]
        //public IActionResult Create(Product product, IFormFile? mainImage, List<string>? UploadedAdditionalImages)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Brands = _brandRepository.GetAll()
        //            .Select(b => new SelectListItem { Value = b.Id, Text = b.Name }).ToList();

        //        ViewBag.Categories = _categoryRepository.GetAll()
        //            .Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList();

        //        return View(product);
        //    }

        //    // Ảnh chính
        //    if (mainImage != null && mainImage.Length > 0)
        //    {
        //        product.Image = _uploadService.UploadFile(mainImage, "uploads/products");
        //    }
        //    else
        //    {
        //        product.Image = "";
        //    }

        //    // Ảnh phụ từ Dropzone
        //    product.Images = UploadedAdditionalImages ?? new List<string>();

        //    _productRepository.Insert(product);
        //    return RedirectToAction(nameof(Index));
        //}



        [HttpPost]
        public IActionResult UploadMultipleImages(IFormFile additionalImages)
        {
            if (additionalImages != null && additionalImages.Length > 0)
            {
                var path = _uploadService.UploadFile(additionalImages, "uploads/products");
                return Json(new { path });
            }

            return BadRequest(new { error = "Upload thất bại" });
        }

        [AuthorizePermission("Product", "Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Brands = _brandRepository.GetAll()
                .Select(b => new SelectListItem
                {
                    Value = b.Id,
                    Text = b.Name
                }).ToList();

            ViewBag.Categories = _categoryRepository.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();

            return View(product);
        }





        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? mainImage, List<IFormFile>? additionalImages, List<string>? additionalImagesList)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns if validation fails
                ViewBag.Brands = _brandRepository.GetAll()
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id,
                        Text = b.Name
                    }).ToList();

                ViewBag.Categories = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    }).ToList();

                return View(product);
            }

            var oldProduct = _productRepository.GetById(product.Id);
            if (oldProduct == null)
            {
                return NotFound();
            }

            // Xử lý ảnh chính
            if (mainImage != null && mainImage.Length > 0)
            {
                var img = _uploadService.UploadFile(mainImage, "uploads/products");
                product.Image = img;
            }
            else
            {
                product.Image = oldProduct.Image;
            }

            // Xử lý ảnh phụ
            product.Images = new List<string>();

            // Giữ lại các ảnh cũ từ additionalImagesList
            if (additionalImagesList != null && additionalImagesList.Any())
            {
                product.Images.AddRange(additionalImagesList);
            }

            // Thêm các ảnh mới được upload
            if (additionalImages != null && additionalImages.Any())
            {
                foreach (var file in additionalImages)
                {
                    if (file != null && file.Length > 0)
                    {
                        var imgPath = _uploadService.UploadFile(file, "uploads/products");
                        product.Images.Add(imgPath);
                    }
                }
            }

            var result = _productRepository.Update(product);
       

            if (result)
            {
                TempData["success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật thất bại.");

                // Repopulate dropdowns if update fails
                ViewBag.Brands = _brandRepository.GetAll()
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id,
                        Text = b.Name
                    }).ToList();

                ViewBag.Categories = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id,
                        Text = c.Name
                    }).ToList();
                TempData["error"] = "Cập nhật sản phẩm thất bại!";
                return View(product);
            }
        }

        [AuthorizePermission("Product", "Delete")]

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm để xoá!";
                return NotFound();
            }
            _productRepository.Delete(id);
            TempData["success"] = "Xoá sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }



        [AuthorizePermission("Product", "Detail")]
        [HttpGet]
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID sản phẩm không hợp lệ.");
            }

            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Lấy thông tin category và brand
            var category = _categoryRepository.GetById(product.CategoryId);
            var brand = _brandRepository.GetById(product.BrandId);

            // Chuyển đổi sang ViewModel
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = category?.Name ?? "Không xác định",
                BrandName = brand?.Name ?? "Không xác định",
                Image = product.Image,
                Images = product.Images ?? new List<string>(),
                CreatedAt = product.CreatedAt
            };

            return View(viewModel);
        }

    }
}
