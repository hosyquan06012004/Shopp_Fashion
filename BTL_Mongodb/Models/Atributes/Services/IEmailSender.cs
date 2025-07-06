namespace BTL_Mongodb.Models.Atributes.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message); //hàm gửi email
    }
}
