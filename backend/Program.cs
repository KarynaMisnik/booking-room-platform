using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args); // Data registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
if (string.IsNullOrEmpty(
    Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5000");
}
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Add services to the container.

builder.Services.AddScoped<BookingValidator>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddControllersWithViews();
// automatically validates antiforgery tokens for requests that can modify application state.
builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Registers the authentication mechanism
    .AddCookie(options =>
    {
        options.Cookie.Name = "HotelStaffAuth"; // Defines the name of authentication cookie
        options.Cookie.HttpOnly = true; // Prevents JavaScript from reading the cookie
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization(); // Enables permission and role checks

// CSRF (Cross-Site Request Forgery) enabled
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// data called from Data.DbInitializer.cs

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync(); // runs in every environment.

    if (app.Environment.IsDevelopment()) // runs only in Development
    {
        await DbInitializer.InitializeAsync(db);
    }
}

app.Run();
