using Microsoft.OpenApi.Models;
using OrderManagementSystem.Services;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Extensions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OrderManagementSystem;

// Make Program class public and partial for testing
public partial class Program
{
    public static void Main(string[] args)
    {

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Maintain reference handling to avoid circular references
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Register repositories
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IDiscountRepository, DiscountRepository>();

// Register services
builder.Services.AddSingleton<IDiscountService, DiscountService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

// Add memory caching
builder.Services.AddMemoryCache();
builder.Services.Decorate<IDiscountService, CachedDiscountService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order Management System API", Version = "v1" });
    
    // Set the comments path for the Swagger JSON and UI
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Enable XML documentation file generation
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management System API v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger path
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

        app.Run();
    }
}
