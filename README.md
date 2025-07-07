*CÔNG NGHỆ SỬ DỤNG CHÍNH: .Net, Mongodb, Frontend cơ bản và jquery

* hưỡng dẫn chạy trang web đầu tiên. 
   1. download và mở project
   2. cài đặt mongodb: mongod server, mongoshell, mongodb database tool và cài đặt biến môi trường     
      + mở cmd (terminal) restore file database mongodb : với  thư muc dbs_mongodb\DB_BTL_Mongodb VD:
      + mongorestore --db DB_BTL_Mongodb "D:\dbs_mongodb\DB_BTL_Mongodb"
      + ở file appsetting.json  kiểm tra và sửa đổi nếu cần.
        "MongoDbSettings": {
           "ConnectionString": "mongodb://127.0.0.1:27017",
           "DatabaseName": "DB_BTL_Mongodb"
         },
       + ở  cmd tiếp tục chạy chạy lệnh mongod
   4. Chạy project: mở terminal trong thư mục project chạy lệnh:
       dotnet restore.
       dotnet run.
   
   5. để đăng nhập bằng admin sử dụng tài khoản:
        tên tài khoản: admin
        mật khẩu : 123456

* một số chức năng đáng chú ý và cách sử dụng
  1. thêm sản phẩm vào giỏ hàng và thanh toán bằng ví vnpay với môi trường test,
       bạn có thể đi tới đường link để lấy tài khoản ảo (lưu ý: nhập đúng tên ngân hàng , số tài khoản, tên tài khoản):
        https://sandbox.vnpayment.vn/apis/vnpay-demo/
  3. đăng nhập nhanh bằng google cho trang người dùng thay vì phải đăng ký rồi đăng nhập
  4. phần quyền cho vai trò người dùng ở admin
     + bạn có thể thêm quyền rồi sau đó gán quyền đó cho vai trò
     + bạn có thể thêm các vài trò rồi gán những vai trò đó cho tài khoản người dùng
  6. đặt hàng có thông báo gửi email và khi cập nhập lại trạng thái cho đơn hàng cũng sẽ gửi email (lưu ý nhập đúng email của ban để nhận thông báo)
 
  * MỘT SỐ HÌNH ẢNH THAM KHẢO:
    + user:
       ![image](https://github.com/user-attachments/assets/127f1e0d-e0ad-4f31-9a88-7085596c0e30)
   
       ![image](https://github.com/user-attachments/assets/69bad5c1-8eaf-4f3e-8e98-be1717ee28e2)
   
       ![image](https://github.com/user-attachments/assets/2c0b0f6b-1c5d-42d9-93d3-3f5759908cb6)
       
       ![image](https://github.com/user-attachments/assets/7aa30f7f-dc67-4355-8189-aa18636cd73a)
       
       ![image](https://github.com/user-attachments/assets/0accf248-6de1-41b0-b4cf-4a50d3370705)
       
       ![image](https://github.com/user-attachments/assets/924eca8f-e559-4154-845a-60fd02e92717)
  
    admin:

       ![image](https://github.com/user-attachments/assets/500d4f0f-f258-4f0b-bb18-d7657c5ff7be)
       
       ![image](https://github.com/user-attachments/assets/0c6423ec-5311-4316-bf50-1e2ba0063de5)
   
       ![image](https://github.com/user-attachments/assets/0c953ae9-4f1b-4c6f-a394-2060b4141d13)
           chi tiết sản phẩm có thể xem các sản phẩm liên quan và zoom ảnh để xem chi tiết rõ hơn :
              ![image](https://github.com/user-attachments/assets/0d9903bb-8e63-4680-87b9-a68f96ca2ac6)
    
       ![image](https://github.com/user-attachments/assets/252f0b21-afae-477c-ab72-2b8673a64956)
    
       ![image](https://github.com/user-attachments/assets/a88dd63d-027c-4d5d-826a-443e45fa91d7)
    
       ![image](https://github.com/user-attachments/assets/143997f3-5c0b-45af-8d20-ebc37354b547)
    
       ![image](https://github.com/user-attachments/assets/1474d363-c513-4155-80fd-a13b7d1b6c9f)








    





