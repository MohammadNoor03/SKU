using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SKU.DbContexts;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!); // Define the secret key
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // The Source Where The Token Is Issued
            ValidateAudience = false, // The Users Whom Can Use This Token
            ValidateIssuerSigningKey = true, // Make Sure That The Token Is Using My Secret Key
            IssuerSigningKey = new SymmetricSecurityKey(key), // Generate The Token Using Our Key
        };
    });

// Container : Dependency Injection
builder.Services.AddDbContext<SKUContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SKUContext"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // لازم قبل UseAuthorization

app.UseAuthorization();

app.MapControllers();

app.Run();