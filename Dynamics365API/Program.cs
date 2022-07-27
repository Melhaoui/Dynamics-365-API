using Dynamics365API.Dtos;
using Dynamics365API.Helpers;
using Dynamics365API.Models;
using Dynamics365API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//JWT
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

//auth and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //Confirmed Email
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICrmService, CrmService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<CRM, CRM>();


//Connect DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });
//Services SMTP Email
builder.Services.Configure<SMTP>(builder.Configuration.GetSection("SMTP"));

//Enable CORS 
builder.Services.AddCors();
//var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
//builder.Services.AddScoped<IEmailSender, EmailSender>();
//builder.Services.AddSingleton(emailConfig);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Auth
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//Enable CORS 
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();
