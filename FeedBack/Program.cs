using FeedBack.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FeedBackContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite");

builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });


builder.Services.AddSwaggerGen(sw =>
{
    sw.AddSecurityDefinition("Authentication Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    sw.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Authentication Bearer"
                },
            },
            Array.Empty<string>()
        }
    });
});

Console.WriteLine($"Database path: {builder.Configuration.GetConnectionString("DefaultConnection")}");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
