using System.Text;
using ApplicationDatabaseContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestCrafter.AutoMapperProfile;
using QuestCrafter.Models;

var builder = WebApplication.CreateBuilder(args /*, new WebApplicationOptions{WebRootPath = "/if/want/to/specify/webrootpath"}*/);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding dbcontext
builder.Services.AddDbContext<AppDbCtx> (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbCtx>()
    .AddDefaultTokenProviders();


// adding Auth
var key = builder.Configuration["jwt:key"] ?? throw new ArgumentNullException("jtw:key is missing");
builder.Services.AddAuthentication(opt =>{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>{
    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,              // Check who issued the token
        ValidateAudience = true,           // Check who it’s for
        ValidateLifetime = true,           // Check if it’s expired
        ValidateIssuerSigningKey = true,   // Check the signature
        ValidIssuer = builder.Configuration["Jwt:Issuer"],     // e.g., "MyApp"
        ValidAudience = builder.Configuration["Jwt:Audience"], // e.g., "MyAppUsers"
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(key)) // Secret key
    };
});

// adding automapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// custom rules; add them if you feel the need.
builder.Services.Configure<IdentityOptions>(CustomIdntityRules.CustomIdentityConfig);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
