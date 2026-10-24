using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models;

var builder = WebApplication.CreateBuilder(args); // Data registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hotelbooking.db"));
builder.WebHost.UseUrls("http://localhost:5000");
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

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();

// seed code

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Rooms.Any())
    {
        db.Rooms.AddRange(
            new Room
            {
                Name = "Standard Room",
                Price = 89.00m,
                Capacity = 2
            },
            new Room
            {
                Name = "Deluxe Room",
                Price = 129.00m,
                Capacity = 3
            }
        );

        db.SaveChanges();
    }
}

app.Run();
