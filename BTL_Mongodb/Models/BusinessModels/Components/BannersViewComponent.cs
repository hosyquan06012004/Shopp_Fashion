using Microsoft.AspNetCore.Mvc;
using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using BTL_Mongodb.Models.Data;

namespace BTL_Mongodb.Models.BusinessModels.Components
{
    public class BannerViewComponent : ViewComponent 
    {
        private readonly IRepositoryBanner _bannerRepo;

        public BannerViewComponent(IRepositoryBanner bannerRepo)
        {
            _bannerRepo = bannerRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy toàn bộ danh sách banner từ Repository
            var banners = _bannerRepo.GetAll();

            return View(banners); // Trả về View kèm danh sách banner
        }
    }
}
