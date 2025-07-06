namespace BTL_Mongodb.Models.Atributes
{
    public class Upload
    {
        public string UploadFile(IFormFile file, string subFolder = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            // Tạo thư mục nếu chưa tồn tại
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Lấy tên file và lưu
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // Trả về đường dẫn liên quan
            return "/" + Path.Combine(subFolder, fileName).Replace("\\", "/");
        }
    }
}
