* hưỡng dẫn chạy trang web đầu tiên. 
1. download và mở project
2. cài đặt mongodb: mongod server, mongoshell, mongodb database tool và cài đặt biến môi trường
   restore file database mongodb : mongorestore --db DB_BTL_Mongodb với thư muc dbs_mongodb\DB_BTL_Mongodb VD: "D:\dbs_mongodb\DB_BTL_Mongodb".
   ở file appsetting.json  thay đổi nếu cần để phù hợp với đường dẫn database mongodb
     "MongoDbSettings": {
        "ConnectionString": "mongodb://127.0.0.1:27017",
        "DatabaseName": "DB_BTL_Mongodb"
      },
3. để đăng nhập bằng admin sử dụng tài khoản:
     tên tài khoản: admin
     mật khẩu : 123456

* một số chức năng đáng chú ý
  1. thêm sản phẩm vào giỏ hàng và thanh toán bằng ví vnpay với môi trường test
       bạn có thể đi tới đường link để lấy tài khoản ảo: https://sandbox.vnpayment.vn/apis/vnpay-demo/
  2. đăng nhập nhanh bằng google cho trang người dùng thay vì phải đăng ký rồi đăng nhập
  3. phần quyền cho vai trò người dùng ở admin 
