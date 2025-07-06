using System.ComponentModel.DataAnnotations;

namespace BTL_Mongodb.Models.ViewModel
{
    public class CheckoutViewModel
    {
        
            [Required(ErrorMessage = "Vui lòng nhập họ")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập tên")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập quốc gia")]
            public string Country { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
            public string Address { get; set; }

            public string Address2 { get; set; } // Không bắt buộc

            [Required(ErrorMessage = "Vui lòng nhập thành phố")]
            public string City { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành phố")]
            public string State { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mã bưu điện")]
            public string ZipCode { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập email"), EmailAddress]
            public string Email { get; set; }

            //[Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
            //public string PaymentMethod { get; set; } 

            public bool HasNote { get; set; }

            public string? Note { get; set; }

 
    }
}