using System.ComponentModel.DataAnnotations;
namespace BTL_Mongodb.Models.Validations
{

    public class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true; // để phòng trường hợp không có giá trị, bạn xử lý Required riêng
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue <= DateTime.Now;
            }
            return false;
        }
    }

}
