using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using BusinessObjects;
using DataAccessObjects;
using FUNewsManagementSystemAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Configure OData
builder
    .Services.AddControllers()
    .AddOData(opt =>
    {
        opt.AddRouteComponents("odata", GetEdmModel())
            .Select()
            .Filter()
            .Expand()
            .OrderBy()
            .SetMaxTop(100)
            .Count()
            .SkipToken();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

// Configure Authentication & Authorization
builder
    .Services.AddAuthentication(options =>
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
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])
            ),
        };
    });

string adminEmail = configuration["AdminAccount:Email"];

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy =>
            policy.RequireAssertion(context =>
            {
                var emailClaim =
                    context.User.FindFirst(ClaimTypes.Email) ?? context.User.FindFirst("email");
                return emailClaim != null
                    && emailClaim.Value.Equals(adminEmail, StringComparison.OrdinalIgnoreCase);
            })
    );

    options.AddPolicy("StaffOnly", policy => policy.RequireClaim("Role", "1"));

    options.AddPolicy("LecturerOnly", policy => policy.RequireClaim("Role", "2"));

    options.AddPolicy(
        "AdminOrStaffOrLecturer",
        policy =>
            policy.RequireAssertion(context =>
            {
                var emailClaim =
                    context.User.FindFirst(ClaimTypes.Email) ?? context.User.FindFirst("email");
                var roleClaim = context.User.FindFirst("Role");

                bool isAdmin =
                    emailClaim != null
                    && emailClaim.Value.Equals(adminEmail, StringComparison.OrdinalIgnoreCase);
                bool isStaffOrLecturer =
                    roleClaim != null && (roleClaim.Value == "1" || roleClaim.Value == "2");

                return isAdmin || isStaffOrLecturer;
            })
    );
});

// Register application services
builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "FU News Management API",
            Version = "v1",
            Description =
                "API for managing news articles, categories, tags, and user accounts. "
                + "Includes both traditional REST endpoints and OData endpoints at /odata/",
        }
    );

    // Resolve conflicting actions by taking the first one
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Configure Swagger to handle OData controllers better
    c.DocInclusionPredicate(
        (docName, apiDesc) =>
        {
            // Include all endpoints but prioritize non-OData for main documentation
            return true;
        }
    );

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "JWT Authentication for News Management System",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } }
    );
});

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FU News Management API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "FU News Management API";
        c.DefaultModelExpandDepth(2);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}

// Enable static files in wwwroot
app.UseStaticFiles();

// Root redirect
app.MapGet(
    "/",
    context =>
    {
        context.Response.Redirect("/login.html");
        return Task.CompletedTask;
    }
);

// OData information endpoint
app.MapGet(
    "/odata",
    async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(
            @"
<!DOCTYPE html>
<html>
<head>
    <title>OData API Information</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: #f8f9fa; }
        .container { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); max-width: 800px; margin: 0 auto; }
        h1 { color: #333; border-bottom: 2px solid #007bff; padding-bottom: 10px; }
        .endpoint { background: #f8f9fa; padding: 15px; margin: 10px 0; border-radius: 5px; border-left: 4px solid #007bff; }
        .method { font-weight: bold; color: #007bff; margin-bottom: 5px; }
        code { background: #e9ecef; padding: 2px 6px; border-radius: 3px; font-family: 'Courier New', monospace; }
        .link { color: #007bff; text-decoration: none; }
        .link:hover { text-decoration: underline; }
        ul { line-height: 1.6; }
        .warning { background: #fff3cd; border: 1px solid #ffeaa7; padding: 10px; border-radius: 5px; margin: 15px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🚀 FU News Management OData API</h1>
        
        <div class='warning'>
            <strong>⚠️ Authentication Required:</strong> All requests need JWT Bearer token in Authorization header.
            Get token from <code>POST /api/Auth/login</code>
        </div>
        
        <h2>📋 Available Entity Sets</h2>
        
        <div class='endpoint'>
            <div class='method'>📰 NewsArticles</div>
            <code>/odata/NewsArticles</code><br>
            Full CRUD operations for news articles with relationships to Categories and Authors.
        </div>
        
        <div class='endpoint'>
            <div class='method'>📁 Categories</div>
            <code>/odata/Categories</code><br>
            Manage news categories with hierarchical parent-child relationships.
        </div>
        
        <div class='endpoint'>
            <div class='method'>🏷️ Tags</div>
            <code>/odata/Tags</code><br>
            Create and manage tags for organizing news articles.
        </div>
        
        <div class='endpoint'>
            <div class='method'>👥 SystemAccounts</div>
            <code>/odata/SystemAccounts</code><br>
            User account management (Admin access only).
        </div>
        
        <h2>🔍 OData Query Examples</h2>
        <ul>
            <li><code>/odata/NewsArticles?$filter=NewsStatus eq true</code> - Published articles only</li>
            <li><code>/odata/NewsArticles?$expand=Category,CreatedBy</code> - Include related data</li>
            <li><code>/odata/NewsArticles?$orderby=CreatedDate desc</code> - Sort by date</li>
            <li><code>/odata/NewsArticles?$top=10&$skip=20</code> - Pagination</li>
            <li><code>/odata/NewsArticles?$count=true</code> - Include total count</li>
            <li><code>/odata/NewsArticles?$select=NewsTitle,CreatedDate</code> - Select specific fields</li>
            <li><code>/odata/Categories?$filter=contains(CategoryName,'tech')</code> - Text search</li>
        </ul>
        
        <h2>🔗 Useful Links</h2>
        <ul>
            <li><a href='/odata/$metadata' class='link'>📊 OData Metadata</a> - Complete schema definition</li>
            <li><a href='/swagger' class='link'>📖 Swagger Documentation</a> - Interactive API docs</li>
            <li><a href='/login.html' class='link'>🔐 Login Page</a> - Get authentication token</li>
        </ul>
        
        <h2>🎯 CRUD Operations</h2>
        <ul>
            <li><strong>Create:</strong> <code>POST /odata/EntitySet</code></li>
            <li><strong>Read:</strong> <code>GET /odata/EntitySet</code> or <code>GET /odata/EntitySet(key)</code></li>
            <li><strong>Update:</strong> <code>PUT /odata/EntitySet(key)</code> or <code>PATCH /odata/EntitySet(key)</code></li>
            <li><strong>Delete:</strong> <code>DELETE /odata/EntitySet(key)</code></li>
        </ul>
    </div>
</body>
</html>"
        );
    }
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    // Configure entity sets
    builder.EntitySet<NewsArticle>("NewsArticles");
    builder.EntitySet<SystemAccount>("SystemAccounts");
    builder.EntitySet<Tag>("Tags");
    builder.EntitySet<Category>("Categories");

    // Configure entity keys explicitly
    builder.EntityType<NewsArticle>().HasKey(x => x.NewsArticleId);
    builder.EntityType<SystemAccount>().HasKey(x => x.AccountId);
    builder.EntityType<Tag>().HasKey(x => x.TagId);
    builder.EntityType<Category>().HasKey(x => x.CategoryId);

    // Optional: Configure navigation properties
    builder.EntityType<NewsArticle>().HasOptional(x => x.Category);

    builder.EntityType<NewsArticle>().HasOptional(x => x.CreatedBy);

    builder.EntityType<Category>().HasOptional(x => x.ParentCategory);

    return builder.GetEdmModel();
}
