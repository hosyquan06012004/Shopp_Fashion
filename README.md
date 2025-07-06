*CÔNG NGHỆ SỬ DỤNG CHÍNH: .Net, Mongodb, Frontend cơ bản và jquery

* hưỡng dẫn chạy trang web đầu tiên. 
   1. download và mở project
   2. cài đặt mongodb: mongod server, mongoshell, mongodb database tool và cài đặt biến môi trường     
      + restore file database mongodb : với  thư muc dbs_mongodb\DB_BTL_Mongodb VD:
      + mongorestore --db DB_BTL_Mongodb "D:\dbs_mongodb\DB_BTL_Mongodb"
      ở file appsetting.json  kiểm tra và sửa đổi nếu cần.
        "MongoDbSettings": {
           "ConnectionString": "mongodb://127.0.0.1:27017",
           "DatabaseName": "DB_BTL_Mongodb"
         },
       - mở cmd lên chạy lệnh mongod
   4. Chạy project: mở terminal trong thư mục project chạy lệnh:
       dotnet restore.
       dotnet run.
   
   5. để đăng nhập bằng admin sử dụng tài khoản:
        tên tài khoản: admin
        mật khẩu : 123456

* một số chức năng đáng chú ý
  1. thêm sản phẩm vào giỏ hàng và thanh toán bằng ví vnpay với môi trường test,
       bạn có thể đi tới đường link để lấy tài khoản ảo (lưu ý: nhập đúng tên ngân hàng , số tài khoản, tên tài khoản):
        https://sandbox.vnpayment.vn/apis/vnpay-demo/
  3. đăng nhập nhanh bằng google cho trang người dùng thay vì phải đăng ký rồi đăng nhập
  4. phần quyền cho vai trò người dùng ở admin 
  5. đặt hàng có thông báo gửi email và khi cập nhập lại trạng thái cho đơn hàng cũng sẽ gửi email (lưu ý nhập đúng email của ban để nhận thông báo)
