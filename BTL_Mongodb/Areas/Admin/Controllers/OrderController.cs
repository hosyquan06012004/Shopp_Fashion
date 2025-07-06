// OrderController.cs
using BTL_Mongodb.Models;
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using BTL_Mongodb.Models.Atributes.Services;
namespace BTL_Mongodb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizePermission("Order", "View")]
    public class OrderController : Controller
    {
        private const int DefaultPageSize = 5;
        private readonly IRepositoryOrder _orderRepository;
        private readonly IEmailSender _emailSender;

        public OrderController(IRepositoryOrder orderRepository, IEmailSender emailSender)
        {
            _orderRepository = orderRepository;
            _emailSender = emailSender;
        }


        public IActionResult Index(OrderSearchModel searchModel, int page = 1, int pageSize = 0)
        {
            if (pageSize <= 0) pageSize = DefaultPageSize;
            IPagedList<OrderViewModel> model;

            bool hasSearch = !string.IsNullOrEmpty(searchModel.CustomerName) ||
                            !string.IsNullOrEmpty(searchModel.Email) ||
                            !string.IsNullOrEmpty(searchModel.Phone) ||
                            searchModel.FromDate.HasValue ||
                            searchModel.ToDate.HasValue ||
                            searchModel.MinAmount.HasValue ||
                            searchModel.MaxAmount.HasValue;

            if (hasSearch)
            {
                model = _orderRepository.Search(searchModel, page, pageSize);
            }
            else
            {
                model = _orderRepository.GetFull(page, pageSize);
            }

            ViewBag.PageSize = pageSize;
            ViewBag.SearchModel = searchModel;
            return View(model);
        }

        [AuthorizePermission("Order", "Edit")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(_orderRepository.ConvertToViewModel(order));
        }

        // Trong action Edit của Controller
        //[HttpPost]
        //public async Task<IActionResult> Edit(OrderViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Lấy đơn hàng hiện tại từ database
        //        var order = _orderRepository.GetById(model.Id);
        //        if (order == null)
        //        {
        //            return NotFound();
        //        }

        //        // Lưu lại trạng thái cũ để kiểm tra thay đổi
        //        var oldStatus = order.Status;
        //        var statusChanged = oldStatus != model.Status;

        //        // Cập nhật các trường
        //        order.FirstName = model.FullName.Split(' ')[0];
        //        order.LastName = model.FullName.Split(' ').Length > 1 ? string.Join(" ", model.FullName.Split(' ').Skip(1)) : "";
        //        order.Address = model.Address;
        //        order.Phone = model.Phone;
        //        order.Email = model.Email;
        //        order.Status = model.Status;

        //        // Lưu thay đổi
        //        var result = _orderRepository.Update(order);

        //        if (result)
        //        {
        //            // Gửi email nếu trạng thái thay đổi
        //            if (statusChanged)
        //            {
        //                await SendStatusChangeEmail(order, oldStatus);
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }

        //    // Nếu có lỗi, hiển thị lại form với dữ liệu cũ
        //    return View(model);
        //}
        [HttpPost]
        public async Task<IActionResult> Edit(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = _orderRepository.GetById(model.Id);
            if (order == null)
            {
                TempData["error"] = "ID không khớp khi cập nhật đơn hàng.";
                return NotFound();
            }

            // Lưu lại trạng thái cũ để kiểm tra thay đổi
            var oldStatus = order.Status;

            // Cập nhật các trường (dù có thay đổi hay không)
            order.FirstName = model.FullName.Split(' ')[0];
            order.LastName = model.FullName.Split(' ').Length > 1 ? string.Join(" ", model.FullName.Split(' ').Skip(1)) : "";
            order.Address = model.Address;
            order.Phone = model.Phone;
            order.Email = model.Email;
            order.Status = model.Status;

            // Luôn thực hiện cập nhật (không kiểm tra hasChanges)
            try
            {
                var result = _orderRepository.Update(order);

                // Gửi email nếu trạng thái thay đổi
                if (oldStatus != order.Status)
                {
                    await SendStatusChangeEmail(order, oldStatus);
                }

                TempData["success"] = "Đơn hàng đã được cập nhật";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                TempData["error"] = "Cập nhật đơn hàng thất bại!";
                return View(model);
            }
        }
        private async Task SendStatusChangeEmail(Order order, string oldStatus)
        {
            var statusText = GetStatusText(order.Status);
            var oldStatusText = GetStatusText(oldStatus);

            string subject = $"Cập nhật trạng thái đơn hàng #{order.Id}";
            string message = $@"
                <h3>Xin chào {order.FirstName} {order.LastName},</h3>
                <p>Trạng thái đơn hàng #{order.Id} của bạn đã được cập nhật:</p>
                <p><strong>Từ:</strong> {oldStatusText}</p>
                <p><strong>Thành:</strong> {statusText}</p>
                <p><strong>Tổng số tiền:</strong> {order.TotalAmount:N0} VND</p>
                <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                <p>Trân trọng,</p>
                <p>Đội ngũ hỗ trợ</p>";

            await _emailSender.SendEmailAsync(order.Email, subject, message);
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xử lý",
                "Processing" => "Đang xử lý",
                "Paid" => "Đã thanh toán",
                "Completed" => "Hoàn thành",
                "Cancelled" => "Đã hủy",
                _ => status
            };
        }

        [AuthorizePermission("Order", "Delete")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng cần xóa.";
                return NotFound();
            }

            bool isDeleted = _orderRepository.Delete(id);
            if (isDeleted)
            {
                TempData["success"] = "Xóa đơn hàng thành công!";
            }
            else
            {
                TempData["error"] = "Xóa đơn hàng thất bại!";
            }

            return RedirectToAction(nameof(Index));
        }

        [AuthorizePermission("Order", "Detail")]
        [HttpGet]
        public IActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID đơn hàng không hợp lệ.");
            }

            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                TempData["error"] = "Không tìm thấy đơn hàng.";
                return NotFound("Không tìm thấy đơn hàng.");
            }

            return View(_orderRepository.ConvertToViewModel(order));
        }
    }
}