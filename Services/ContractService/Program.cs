using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Infrastructure;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.ContractsContext.Application.QueriesServices;
using workstation_backend.ContractsContext.Application.CommandServices;
using workstation_backend.ContractsContext.Domain.Models.Validators;
using workstation_backend.ContractsContext.Application.EventServices;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Domain.Repositories; 
using workstation_backend.Shared.Infrastructure.Persistence.Repositories; 

var builder = WebApplication.CreateBuilder(args);

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("JWT key is not set in configuration.");

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contract Service",
        Version = "v1",
        Description = "Microservicio de Contratos"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("Database connection string is not set.");

Console.WriteLine($"Connection String: {connectionString}");

builder.Services.AddDbContext<ContractContext>(options =>
{
    options.UseNpgsql(connectionString)
           .LogTo(Console.WriteLine, LogLevel.Information) // Cambié a Information para ver más logs
           .EnableSensitiveDataLogging() // Para ver los valores en desarrollo
           .EnableDetailedErrors();
});

// Shared Infrastructure
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractCommandService, ContractCommandService>();
builder.Services.AddScoped<IContractQueryService, ContractQueryService>();
builder.Services.AddScoped<IContractEventService, ContractEventService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<AddClauseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddCompensationCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FinishContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SignContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateReceiptCommandValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

app.UseCors("AllowAll");

// Swagger simple y directo
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ CORREGIDO: Crear la base de datos con EnsureCreated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContractContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    var retryCount = 0;
    const int maxRetries = 10;

    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("=== Attempting to setup PostgreSQL database ===");
            logger.LogInformation($"Connection string: {connectionString}");
            
            // Verifica la conexión
            var canConnect = context.Database.CanConnect();
            logger.LogInformation($"Can connect to PostgreSQL: {canConnect}");
            
            if (!canConnect)
            {
                throw new Exception("Cannot connect to PostgreSQL");
            }
            
            // Crea la base de datos y tablas
            logger.LogInformation("Creating database and tables...");
            var created = context.Database.EnsureCreated();
            
            if (created)
            {
                logger.LogInformation("✅ PostgreSQL database CREATED successfully.");
            }
            else
            {
                logger.LogInformation("✅ PostgreSQL database already EXISTS.");
            }
            
            // Verifica que las tablas existan
            var tables = context.Model.GetEntityTypes().Select(t => t.GetTableName()).ToList();
            logger.LogInformation($"Expected tables: {string.Join(", ", tables)}");
            
            // Prueba una consulta simple
            var contractCount = context.Contracts.Count();
            logger.LogInformation($"Contract count: {contractCount}");
            
            logger.LogInformation("=== PostgreSQL setup completed successfully ===");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogError($"❌ Database setup attempt {retryCount}/{maxRetries} failed");
            logger.LogError($"Error: {ex.Message}");
            logger.LogError($"Stack trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }

            if (retryCount == maxRetries)
            {
                logger.LogCritical("Failed to setup database after maximum retries.");
                throw;
            }

            logger.LogInformation($"Waiting 5 seconds before retry...");
            Thread.Sleep(5000);
        }
    }
}

app.Run();