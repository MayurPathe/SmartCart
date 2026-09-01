using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// JWT CONFIGURATION
// ============================================================

var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtSecretKey = jwtSection["SecretKey"];

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "JWT Issuer is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT Audience is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtSecretKey))
{
    throw new InvalidOperationException(
        "JWT SecretKey is not configured.");
}

// ============================================================
// AUTHENTICATION
// ============================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Validate Issuer
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                // Validate Audience
                ValidateAudience = true,
                ValidAudience = jwtAudience,

                // Validate JWT signature
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecretKey)),

                // Validate expiration
                ValidateLifetime = true,

                // Don't allow extra time
                ClockSkew = TimeSpan.Zero
            };
    });


// ============================================================
// AUTHORIZATION
// ============================================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});


// ============================================================
// YARP REVERSE PROXY
// ============================================================

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


// ============================================================
// YARP ROUTES
// ============================================================

app.MapReverseProxy();
// ============================================================
// RUN
// ============================================================

app.Run();

//app.MapGet("/", () => "Hello World!");

//app.Run();
