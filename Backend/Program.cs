using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// Register your DbContext using the connection string
builder.Services.AddDbContext<RentalDbContext>(options =>
{
    options.UseMySql(
        configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(10, 5, 12)), // Specify the server version here
        mySqlOptions =>
        {
            // Configure charset behavior here if needed
        }
    );
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
