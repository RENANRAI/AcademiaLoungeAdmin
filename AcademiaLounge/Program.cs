using System.Text;
using AcademiaLounge.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<AcademiaLounge.Security.JwtTokenService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AcademiaLounge API", Version = "v1" });

    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString("1985-08-30")
    });

    c.MapType<DateOnly?>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Nullable = true,
        Example = new OpenApiString("1985-08-30")
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole APENAS o token JWT (sem 'Bearer ')."
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

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var jwt = builder.Configuration.GetSection("Jwt");

var key = jwt["Key"]?.Trim();
if (string.IsNullOrWhiteSpace(key))
    throw new InvalidOperationException("Jwt:Key não configurado.");

var validIssuer = string.IsNullOrWhiteSpace(jwt["Issuer"]) ? "academia" : jwt["Issuer"]!.Trim();
var validAudience = string.IsNullOrWhiteSpace(jwt["Audience"]) ? "academia" : jwt["Audience"]!.Trim();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.UseSecurityTokenValidators = true;
        // força o validador clássico
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = validIssuer,
            ValidAudience = validAudience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {

                var auth = ctx.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(auth) &&
                    auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = auth.Substring("Bearer ".Length).Trim();
                    ctx.Token = token;
                    ctx.HttpContext.Items["jwt_token"] = token;

                    var issuer = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler()
                                     .ReadJwtToken(token).Issuer;
                    Console.WriteLine("ISS PARSE MANUAL: " + issuer);
                }



                return Task.CompletedTask;
            },

            OnTokenValidated = ctx =>
            {
                var jwtToken = ctx.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
                Console.WriteLine("TOKEN ISSUER LIDO: " + (jwtToken?.Issuer ?? "(null)"));
                Console.WriteLine("TOKEN AUD LIDO: " + (jwtToken?.Audiences?.FirstOrDefault() ?? "(null)"));
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = ctx =>
            {
                var auth = ctx.Request.Headers["Authorization"].ToString();
                var token = ctx.HttpContext.Items["jwt_token"]?.ToString() ?? "";

                Console.WriteLine("JWT FAIL: " + ctx.Exception.Message);
                Console.WriteLine("PATH: " + ctx.Request.Path);
                Console.WriteLine("AUTH HEADER: " + auth);
                Console.WriteLine("TOKEN(SAVED): " + token);
                Console.WriteLine("DOT COUNT: " + token.Count(c => c == '.'));
                Console.WriteLine("EXPECTED ISSUER: " + validIssuer);
                Console.WriteLine("EXPECTED AUDIENCE: " + validAudience);

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
