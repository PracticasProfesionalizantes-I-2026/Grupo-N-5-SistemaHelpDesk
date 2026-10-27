using System.Text;
using HelpDesk.API.Background;
using HelpDesk.API.Middleware;
using HelpDesk.BusinessLogic.Interfaces;
using HelpDesk.BusinessLogic.Services;
using HelpDesk.DataAccess.Data;
using HelpDesk.DataAccess.Interfaces;
using HelpDesk.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e =>
                    {
                        var msg = e.ErrorMessage;
                        var exMsg = e.Exception?.Message ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(msg) && e.Exception != null)
                        {
                            msg = exMsg;
                        }

                        // Número o tipo incorrecto donde se espera String
                        if (msg.Contains("could not be converted to System.String", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted to System.String", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El valor del campo debe ser una cadena de texto (string), no un número ni un objeto.";
                        }

                        // GUID inválido
                        if (msg.Contains("could not be converted to System.Guid", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted to System.Guid", StringComparison.OrdinalIgnoreCase) ||
                            e.Exception is FormatException ||
                            exMsg.Contains("Guid", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El formato del identificador no es válido. Debe ser un GUID válido (ej: 00000000-0000-0000-0000-000000000000).";
                        }

                        // Número entero
                        if (msg.Contains("could not be converted to System.Int", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted to System.Int", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El valor del campo debe ser un número entero válido.";
                        }

                        // Booleano
                        if (msg.Contains("could not be converted to System.Boolean", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted to System.Boolean", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El valor del campo debe ser un booleano (true o false).";
                        }

                        // DateTime
                        if (msg.Contains("could not be converted to System.DateTime", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted to System.DateTime", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El formato de la fecha no es válido.";
                        }

                        // Body vacío
                        if (msg.Contains("non-empty request body", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("non-empty request body", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El cuerpo de la solicitud no puede estar vacío. Debe enviar un JSON con los datos requeridos.";
                        }

                        // JSON malformado
                        if (msg.Contains("JSON", StringComparison.OrdinalIgnoreCase) && 
                            (msg.Contains("syntax", StringComparison.OrdinalIgnoreCase) || msg.Contains("invalid", StringComparison.OrdinalIgnoreCase)))
                        {
                            return "El cuerpo de la solicitud contiene formato JSON inválido o mal estructurado.";
                        }

                        // Conversión genérica de tipo
                        if (msg.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) ||
                            exMsg.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
                        {
                            return "El tipo de dato proporcionado no es compatible con el campo esperado.";
                        }

                        return msg;
                    }).ToArray()
                );

            var firstError = errors.Values.SelectMany(v => v).FirstOrDefault()
                ?? "La solicitud contiene errores de formato o validación.";

            var responseObj = new
            {
                error = new
                {
                    code = "BadRequest",
                    message = firstError,
                    details = errors
                },
                message = firstError
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(responseObj);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configuración de SwaggerGen con soporte para JWT Bearer
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HelpDesk API",
        Version = "v1",
        Description = "API RESTful para el Sistema de Gestión de Tickets HelpDesk - Grupo N° 5"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido del endpoint /api/auth/login. Ejemplo: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// DbContext
builder.Services.AddDbContext<HelpDeskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=helpdesk.db"));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPriorityRepository, PriorityRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IStatusHistoryRepository, StatusHistoryRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<ISolutionRepository, SolutionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

// Services

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISolutionService, SolutionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();

// Hosted Background Services
builder.Services.AddHostedService<TicketEscalationService>();

// Configuración de Autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "HelpDeskSecretKeyParaFirmaDeTokenJwtSeguro2026!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HelpDeskAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HelpDeskClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

// Middleware global de manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Swagger UI clásico
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelpDesk API v1");
        c.RoutePrefix = "swagger";
    });

    // Scalar API Reference
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "HelpDesk API";
        options.Theme = ScalarTheme.BluePlanet;
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }  