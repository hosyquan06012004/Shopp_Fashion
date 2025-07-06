using System.ComponentModel.DataAnnotations;

public enum CategoryStatus
{
    [Display(Name = "Chờ")]
    Cho = 0,

    [Display(Name = "Đã kích hoạt")]
    DaKichHoat = 1,

    [Display(Name = "Đã vô hiệu hóa")]
    DaVoHieuHoa = 2,

    [Display(Name = "Đang xử lý")]
    DangXuLy = 3
}