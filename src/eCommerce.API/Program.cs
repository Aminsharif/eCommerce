using eCommerce.Core.Interfaces;
using eCommerce.Core.Settings;
using eCommerce.Infrastructure.Data;
using eCommerce.Infrastructure.Repositories;
using eCommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using eCommerce.API.Infrastructure.Swagger;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "eCommerce API",
        Version = "v1",
        Description = "API for eCommerce platform",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@ecommerce.com"
        }
    });

    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Include XML comments from the Core project
    var coreXmlFile = "eCommerce.Core.xml";
    var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
    if (File.Exists(coreXmlPath))
    {
        c.IncludeXmlComments(coreXmlPath);
    }

    // Include XML comments from the Infrastructure project
    var infrastructureXmlFile = "eCommerce.Infrastructure.xml";
    var infrastructureXmlPath = Path.Combine(AppContext.BaseDirectory, infrastructureXmlFile);
    if (File.Exists(infrastructureXmlPath))
    {
        c.IncludeXmlComments(infrastructureXmlPath);
    }

    // Use enum values instead of string
    c.UseAllOfToExtendReferenceSchemas();
    c.SchemaFilter<EnumSchemaFilter>();
    c.OperationFilter<SwaggerDefaultValues>();
    
    // Resolve any conflicting actions
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();


// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Logging
builder.Services.AddLogging();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
