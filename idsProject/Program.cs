using Ids.Data;
using Ids.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Identity + Identity API endpoints (register/login/etc.)
builder.Services
    .AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>() //  needed for RoleManager + roles
    .AddEntityFrameworkStores<AppDbContext>();

//  Authorization (so [Authorize] works)

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Default: read from Authorization header.
            // Optional: if you ever store access token in cookie, read it here.
            // Example cookie name: "accessToken"
            var tokenFromCookie = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrWhiteSpace(tokenFromCookie))
            {
                context.Token = tokenFromCookie;
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization();


// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "idsProject", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// AutoMapper (keep your assembly)
builder.Services.AddAutoMapper(cfg => { }, typeof(QuizAttemptProfile).Assembly);


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();


var roleNames = new[] { "Admin", "Instructor", "Student" };

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var roleName in roleNames)
    {
        var exists = await roleManager.RoleExistsAsync(roleName);
        if (!exists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var adminEmail = "admin@local.test";
    var adminPassword = "Admin123!@#"; // dev only

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin",
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Admin user seed failed: {errors}");
        }
    }

    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
            throw new Exception($"Assign Admin role failed: {errors}");
        }
    }

}




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseCors("Frontend");

//  IMPORTANT ORDER
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

// database connection test
app.MapGet("/db-check", async (AppDbContext context) =>
{
    var canConnect = await context.Database.CanConnectAsync();
    return Results.Ok(new { canConnect });
});
app.MapGet("/db-info", (AppDbContext context) =>
{
    var conn = context.Database.GetDbConnection();
    return Results.Ok(new
    {
        dataSource = conn.DataSource,
        database = conn.Database
    });
});




//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await db.Database.MigrateAsync(); // create/update schema

//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var roles = new[] { "Admin", "Instructor", "Student" };
//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//            await roleManager.CreateAsync(new IdentityRole(role));
//    }
//}

app.Run();
