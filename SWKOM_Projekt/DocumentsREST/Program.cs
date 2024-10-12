using DocumentsREST.BL.Services;
using DocumentsREST.DAL;
using DocumentsREST.DAL.Repositories;
using DocumentsREST.Mappings;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;
using System.IO;
using Microsoft.AspNetCore.Http.Features; // Import this for FileInfo

var builder = WebApplication.CreateBuilder(args);

// Configure log4net
var log4NetConfigFile = new FileInfo("log4net.config");
XmlConfigurator.Configure(log4NetConfigFile);

// Configure logging
builder.Logging.ClearProviders(); // Clear default logging providers
// You can add console logging if needed
builder.Logging.AddConsole(); // Optional, you can remove this if using only log4net

// Add services to the container
builder.Services.AddControllers();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(DocumentMappingProfile));

// Register your DbContext with PostgreSQL and add logging for connection success/failure
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()); // Enable more detailed logging for debugging

// Register your repositories and services for Dependency Injection
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();  // Repository injection
builder.Services.AddScoped<IDocumentService, DocumentService>();        // Service injection

// Set maximum file upload size (20 MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20 MB limit
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI",
        policy =>
        {
            policy.WithOrigins("http://localhost")  // The URL of your Web UI
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())  // Optionally, only use Swagger in development
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebUI");  // Apply CORS policy

app.UseAuthorization();     // Ensure authorization middleware is in the pipeline

// Custom logging to check the database connection status
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = LogManager.GetLogger(typeof(Program)); // Use log4net's ILog

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.OpenConnection();  // Try to open a connection
        context.Database.CloseConnection(); // Close after testing the connection

        logger.Info("Successfully connected to the PostgreSQL database.");
    }
    catch (Exception ex)
    {
        logger.Error("An error occurred while connecting to the PostgreSQL database.", ex);
    }
}

app.MapControllers();       // Map controller endpoints

app.Run();
