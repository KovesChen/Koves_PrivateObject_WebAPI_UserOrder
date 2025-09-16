using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Profiles;
using Koves.UserOrder.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 讀取 appsettings.json 的設定
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//資料庫連線
builder.Services.AddDbContext<KCdbContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("WebDatabase")));

//AutoMapper
builder.Services.AddAutoMapper(typeof(UserLstProfile));

//DI 注入
builder.Services.AddScoped<IUserListService, UserListService>();   // 使用者介面
builder.Services.AddScoped<IProductService, ProductService>();   // 商品介面
builder.Services.AddScoped<IOrderService, OrderService>();


// JWT 驗證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  //驗證發行者
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateAudience = true,  //驗證使用者
            ValidAudience = configuration["Jwt:Audience"],
            ValidateLifetime = true,  //驗證是否過期確認
            ClockSkew = TimeSpan.Zero,  // 設定JWT不要有緩衝時間(憑證到期就到期了)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:KEY"]))
        };
    });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
