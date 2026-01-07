using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.API.Application.Services;
using OrderManagement.API.Domain.Interfaces;
using OrderManagement.API.Infrastructure.Data;
using OrderManagement.API.Infrastructure.Repositories;
using OrderManagement.API.Middleware;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Entity Framework Core con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // React común + Vite
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuración de JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = "role",
        NameClaimType = "name"
    };
});

builder.Services.AddAuthorization();

// Registro de Repositorios
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Registro de Servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PedidoService>();

// Configuración de Polly para Circuit Breaker y Retry Policies
var resiliencePipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,              // Abre el circuito si 50% de las peticiones fallan
        SamplingDuration = TimeSpan.FromSeconds(10),  // Ventana de muestreo de 10 segundos
        MinimumThroughput = 5,           // Mínimo 5 peticiones antes de evaluar
        BreakDuration = TimeSpan.FromSeconds(30)      // Circuito abierto por 30 segundos
    })
    .Build();

builder.Services.AddSingleton(resiliencePipeline);

// Configuración de Controllers
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order Management API",
        Version = "v1",
        Description = "API para gestión de pedidos con autenticación JWT"
    });

    // Configuración de autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
        app.Logger.LogInformation("Migraciones aplicadas exitosamente");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error al aplicar migraciones");
    }

    // Crear usuario admin por defecto si no existe
    try
    {
        var adminEmail = "admin@test.com";
        var existingAdmin = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin == null)
        {
            var adminUser = new OrderManagement.API.Domain.Entities.Usuario
            {
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", workFactor: 11),
                Nombre = "Administrador",
                Rol = "Admin",
                FechaCreacion = DateTime.UtcNow
            };

            dbContext.Usuarios.Add(adminUser);
            await dbContext.SaveChangesAsync();

            app.Logger.LogInformation("Usuario admin creado exitosamente - Email: {Email}, Password: Admin123!", adminEmail);
        }
        else
        {
            app.Logger.LogInformation("Usuario admin ya existe");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error al crear usuario admin");
    }
}

// Middleware global de excepciones
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Order Management API iniciada exitosamente");

app.Run();
