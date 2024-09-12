using Web_API.Data;
using Microsoft.EntityFrameworkCore;
using Web_API.Repository.IRepository;
using Web_API.Repository;
using Microsoft.AspNetCore.Identity;
using Web_API.Models;
using Web_API.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Web_API;
using Stripe;
using Web_API.Service;
using System.Text.Json.Serialization;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false
};

builder.Services.AddSingleton(tokenValidationParams);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParams;
    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwtToken"];
            return Task.CompletedTask;
        }
    };
});

// Configure rate limiting options
// builder.Services.AddOptions();
// builder.Services.AddMemoryCache();
// builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
// builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

// // Add rate limiting Services
// builder.Services.AddInMemoryRateLimiting();



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var server = Environment.GetEnvironmentVariable("DBServer") ?? "localhost";
var database = Environment.GetEnvironmentVariable("Database") ?? "Electronix";
var port = Environment.GetEnvironmentVariable("DBPort") ?? "1433";
var user = Environment.GetEnvironmentVariable("DBUser") ?? "sa";
var password = Environment.GetEnvironmentVariable("DBPassword") ?? "#@!76Mohamad612";
var ConnectionString = $"Server={server},{port};Database={database};User={user};Password={password};TrustServerCertificate=True";
// var ConnectionString = "Server=Electronix.mssql.somee.com;Database=Electronix;User=MohamadElectron_SQLLogin_1;Password=r47vitfn4r;TrustServerCertificate=True";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));

builder.Services.Configure<IdentityOptions>(options =>
    {
        // Password requirements
        options.Password.RequireDigit = true; // Requires at least one digit
        options.Password.RequireLowercase = true; // Requires at least one lowercase letter
        options.Password.RequireUppercase = true; // Requires at least one uppercase letter
        options.Password.RequireNonAlphanumeric = true; // Requires at least one non-alphanumeric character
        options.Password.RequiredLength = 8; // Requires a minimum length of 8 characters
    });
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost",
        policy =>
        {
            policy
            .WithOrigins(builder.Configuration.GetSection("Origins").Get<string[]>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;

    options.MinimumSameSitePolicy = SameSiteMode.None;
});


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "jwtToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Here Enter the JWT Token like the following: beater[space] <your token>"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                }
            },
            new string[]
            {

            }
        }
    });
});


// Adding the unit of work to the DI container
builder.Services.AddLogging();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


var app = builder.Build();


if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

// app.UseIpRateLimiting();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// clear()
// fetch("http://192.168.1.6:5148/api/user/login", {
//     method: "POST",
//     mode: "cors", 
//     credentials: "include", 
//     headers: {
//       "Content-Type": "application/json",
//     },
//     body: JSON.stringify({
//       email: "ali@email.com",
//       password: "Pass1234!"
//     })
//   }).then(res => res.json());

// clear()

// fetch("https://192.168.1.9:7278/api/user/login", {
//     method: "POST",
//     mode: "cors", 
//     credentials: "include", 
//     headers: {
//       "Content-Type": "application/json",
//     },
//     body: JSON.stringify({
//       email: "owner@examlpe.com",
//       password: "Pass1234!"
//     })
//   }).then(res => res.json());


// fetch("https://192.168.1.9:7278/api/user/createadmin", {
//     method: "POST",
//     mode: "cors", 
//     credentials: "include", 
//     headers: {
//       "Content-Type": "application/json",
//     },
//     body: JSON.stringify({
//       username: "Admin",
//       email: "newAdmin@examlpe.com",
//      	password: "Pass1234!",
//       confirmPassword: "Pass1234!",
//       phoneNumber: "string",
//       country: "string"
//     })
//   }).then(res => res.json());
