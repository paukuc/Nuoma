using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = builder.Configuration;

// // Register your DbContext using the connection string
// builder.Services.AddDbContext<RentalDbContext>(options =>
// {
//     options.UseSqlServer(
//         "Server=tcp:rentserver.database.windows.net,1433;Initial Catalog=rentDB;Persist Security Info=False;User ID=paulius;Password=Mydatabase1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
// });

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });
// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();
// using (var scope = app.Services.CreateScope()){
//     scope.ServiceProvider.GetRequiredService<RentalDbContext>().Database.Migrate();
// }
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("MyAllowSpecificOrigins"); // Use the CORS policy here
app.MapControllers();
app.UseAuthentication(); // Use authentication
app.UseAuthorization();  // Use authorization


app.Run();
