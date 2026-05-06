using System.Security.Claims;
using System.Text;
using System.Text.Json;
using InfoExchange.Api.Features.Admin;
using InfoExchange.Api.Features.Common;
using InfoExchange.Api.Features.Web;
using InfoExchange.Api.Features.WebAdmin;
using InfoExchange.Api.Infrastructure.Data;
using InfoExchange.Api.Infrastructure.Security;
using InfoExchange.Api.Infrastructure.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddSingleton<IMd5Hasher, Md5Hasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<WebAdminService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<WebService>();

builder.Services.AddControllers(options =>
{
  options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.InvalidModelStateResponseFactory = context =>
  {
    return new BadRequestObjectResult(ApiResponse.InvalidRequest());
  };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
var signingKey = JwtKeyHelper.CreateSigningKey(jwtOptions.PrivateKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier
      };
      options.Events = new JwtBearerEvents
      {
        OnChallenge = async context =>
        {
          context.HandleResponse();
          context.Response.StatusCode = StatusCodes.Status401Unauthorized;
          context.Response.ContentType = "application/json; charset=utf-8";
          var payload = ApiResponse.Unauthorized();
          await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        },
        OnForbidden = async context =>
        {
          context.Response.StatusCode = StatusCodes.Status403Forbidden;
          context.Response.ContentType = "application/json; charset=utf-8";
          var payload = ApiResponse.Forbidden();
          await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
      };
    });

builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", policy =>
  {
    policy.SetIsOriginAllowed(_ => true)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

var uploadRoot = builder.Configuration["FileStorage:UploadRoot"] ?? "uplodes";
var absoluteUploadRoot = Path.Combine(app.Environment.ContentRootPath, uploadRoot);
Directory.CreateDirectory(absoluteUploadRoot);

app.UseStaticFiles(new StaticFileOptions
{
  RequestPath = "/uplodes",
  FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(absoluteUploadRoot)
});

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { state = new { type = "SUCCESS", msg = "Operation successful" }, data = "InfoExchange .NET API running" }));
app.MapControllers();

app.Run();
