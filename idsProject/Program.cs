using Ids.Data;
using Ids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext BEFORE building the app
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger ONLY in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();






//database connection test
app.MapGet("/db-check", async (AppDbContext context) =>
{
    var canConnect = await context.Database.CanConnectAsync();
    return Results.Ok(new { canConnect });
});




// GET /users -- getting all users from the database
app.MapGet("/users", async (AppDbContext _context) =>
{
    var users = await _context.Users.ToListAsync();

    return Results.Ok(users);

});

// GET /user/{id} -- getting a user by his id
app.MapGet("/users/{id}", async (int id, AppDbContext _context) =>
{
    if (id < 1)
    {
        return Results.BadRequest("invalid id");
    }
    var user = await _context.Users.FindAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

// Delete a user
app.MapDelete("/users/{id}", async (int id, AppDbContext _context) =>
{
    if (id < 1)
    {
        return Results.BadRequest("invalid id");
    }
    var user = await _context.Users.FindAsync(id);
    if (user is null)
    {
        return Results.NotFound();
    }
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return Results.NoContent();

});
//Update a existing user
app.MapPut("/users/{id}", async (int id, [FromBody] UpdateUserDto dto, AppDbContext _context) =>
{
    if (id < 1)
    {
        return Results.BadRequest("invalid id");
    }
    var user = await _context.Users.FindAsync(id);
    if (user is null)
    {
        return Results.NotFound();
    }
    if (string.IsNullOrWhiteSpace(dto.FirstName) ||
        string.IsNullOrWhiteSpace(dto.LastName) ||
        string.IsNullOrWhiteSpace(dto.Email))
        return Results.BadRequest("FirstName, LastName, and Email are required.");

    user.FirstName = dto.FirstName;
    user.LastName = dto.LastName;
    user.Email = dto.Email;
    await _context.SaveChangesAsync();
    return Results.Ok(user);

});

//Create a new user
app.MapPost("/users", async (CreateUserDto dto, AppDbContext _context) =>
{
    if (string.IsNullOrWhiteSpace(dto.FirstName) ||
        string.IsNullOrWhiteSpace(dto.LastName) ||
        string.IsNullOrWhiteSpace(dto.Email) ||
        string.IsNullOrWhiteSpace(dto.Password))
        return Results.BadRequest("FirstName, LastName, Email and Password are required.");

    var user = new User
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Password = dto.Password,
        Role="student"
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role
    });
});


app.Run();

//adding the dto

public record CreateUserDto(string FirstName, string LastName, string Email, string Password,string Role);
public record UpdateUserDto(string FirstName, string LastName, string Email);

