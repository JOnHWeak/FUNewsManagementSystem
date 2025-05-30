using DataAccessObjects;
using FUNewsManagementSystemAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Web.Http.OData.Builder;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;



// Các dịch vụ API Controller
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

// Cấu hình Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
    };
});

string adminEmail = configuration["AdminAccount:Email"];

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim(c => c.Type == ClaimTypes.Email) &&
        context.User.FindFirst(c => c.Type == ClaimTypes.Email).Value.Equals(adminEmail, StringComparison.OrdinalIgnoreCase)));


    options.AddPolicy("StaffOnly", policy =>
        policy.RequireClaim("Role", "1"));

    options.AddPolicy("LecturerOnly", policy =>
        policy.RequireClaim("Role", "2"));

    options.AddPolicy("AdminOrStaffOrLecturer", policy =>
        policy.RequireAssertion(context =>
            (
                context.User.HasClaim(c => c.Type == "email") &&
                context.User.FindFirst(c => c.Type == "email").Value.Equals(adminEmail, StringComparison.OrdinalIgnoreCase)
            )
            ||
            (
                context.User.HasClaim(c => c.Type == "Role") &&
                (context.User.FindFirst(c => c.Type == "Role").Value == "1" ||
                 context.User.FindFirst(c => c.Type == "Role").Value == "2")
            )
        ));
});

// Đăng ký các service khác
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "JWT Authentication for Cosmetics Management",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Cho phép truy cập file tĩnh trong wwwroot
app.UseStaticFiles(); app.MapGet("/", context =>
{
    context.Response.Redirect("/login.html");
    return Task.CompletedTask;
});

app.UseAuthentication(); // Bắt buộc có để kích hoạt xác thực
app.UseAuthorization();

app.MapControllers();

app.Run();
