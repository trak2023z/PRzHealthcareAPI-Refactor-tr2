using NLog.Web;
using PRzHealthcareAPIRefactor;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using PRzHealthcareAPIRefactor.Models;
using PRzHealthcareAPIRefactor.Middlewares;
using PRzHealthcareAPIRefactor.Services;

var builder = WebApplication.CreateBuilder(args);

var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("Authentication").Bind(authenticationSettings);

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddDbContext<HealthcareDbContext>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<HealthcareSeeder>();
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllers().AddFluentValidation().AddJsonOptions(x => { x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddCors(op =>
{
    op.AddPolicy("FrontEndClient", builder =>
        builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});
builder.Services.AddMvc();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<HealthcareSeeder>();
seeder.Seed();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "PRzHealthcareAPI");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseCors("FrontEndClient");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
