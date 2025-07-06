using BTL_Mongodb.Models.BusinessModels.ImplementModels;
using BTL_Mongodb.Models.BusinessModels;
using BTL_Mongodb.Models.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using BTL_Mongodb.Models.Atributes.Services;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IVnPayService, VnPayService>();
//kết nối mongodb 
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Đăng ký MongoDbContext
builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var settings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
    return new MongoDbContext(settings);
});
// Add services to the container.
builder.Services.AddControllersWithViews();


// Đăng ký Repository và DbContext dạng Scoped
builder.Services.AddScoped<IRepositoryCategory, RepositoryCategory>();
builder.Services.AddScoped<IRepositoryBrand, RepositoryBrand>();
builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();
builder.Services.AddScoped<IRepositoryOrder, RepositoryOrder>();
builder.Services.AddScoped<IRepositoryBanner, RepositoryBanner>();
//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/AccountUser/Login";   // trang login
//        options.LogoutPath = "/AccountUser/Logout"; // trang logout
//        options.AccessDeniedPath = "/AccountUser/AccessDenied"; // trang bị từ chối
//    });


builder.Services.AddSingleton<AuthService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

//configuration login google
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    //login normal
    options.LoginPath = "/AccountUser/Login";
    options.LogoutPath = "/AccountUser/Logout";
    options.AccessDeniedPath = "/AccountUser/AccessDenied";
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "admin",
      pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    );
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
app.Run();
