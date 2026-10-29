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
            var rawEntries = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToList();

            // Detectar si hay errores específicos de propiedades más allá del error cascada del DTO
            var hasSpecificPropertyErrors = rawEntries.Any(e =>
                e.Key != "dto" && e.Key != "body" && e.Key != "$" && !string.IsNullOrWhiteSpace(e.Key));

            Type? ResolveTargetType(string propertyName)
            {
                var clean = propertyName.TrimStart('$', '.');
                if (string.IsNullOrWhiteSpace(clean)) return null;

                foreach (var param in context.ActionDescriptor.Parameters)
                {
                    var paramType = param.ParameterType;

                    var prop = paramType.GetProperty(clean, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null) return prop.PropertyType;

                    foreach (var ctor in paramType.GetConstructors())
                    {
                        var cp = ctor.GetParameters().FirstOrDefault(p => string.Equals(p.Name, clean, StringComparison.OrdinalIgnoreCase));
                        if (cp != null) return cp.ParameterType;
                    }
                }
                return null;
            }

            var details = new Dictionary<string, string[]>();
            var allTranslated = new List<string>();

            foreach (var (key, entry) in rawEntries)
            {
                if (entry?.Errors == null) continue;

                // Si hay errores específicos en campos internos, omitir el error genérico 'The dto field is required'
                var isDtoRootKey = key == "dto" || key == "body" || key == "$" || string.IsNullOrWhiteSpace(key);
                if (isDtoRootKey && hasSpecificPropertyErrors)
                {
                    continue;
                }

                var cleanKey = key.TrimStart('$', '.');
                var displayKey = string.IsNullOrWhiteSpace(cleanKey) ? "cuerpo" : cleanKey;
                var targetType = ResolveTargetType(cleanKey);
                var underlying = targetType != null ? (Nullable.GetUnderlyingType(targetType) ?? targetType) : null;

                var keyList = new List<string>();

                foreach (var err in entry.Errors)
                {
                    var msg = err.ErrorMessage ?? string.Empty;
                    var exMsg = err.Exception?.Message ?? string.Empty;
                    var innerExMsg = err.Exception?.InnerException?.Message ?? string.Empty;
                    var fullContext = $"{msg} {exMsg} {innerExMsg}".Trim();

                    var isConversionError = fullContext.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) ||
                                           fullContext.Contains("JSON value", StringComparison.OrdinalIgnoreCase) ||
                                           err.Exception is FormatException;

                    string translated;

                    // 1. GUID inválido (por tipo subyacente, convención Id o mención de Guid en contexto)
                    if ((underlying == typeof(Guid) && isConversionError) ||
                        (cleanKey.EndsWith("Id", StringComparison.OrdinalIgnoreCase) && isConversionError) ||
                        cleanKey.Equals("id", StringComparison.OrdinalIgnoreCase) && isConversionError ||
                        fullContext.Contains("System.Guid", StringComparison.OrdinalIgnoreCase) ||
                        fullContext.Contains("Guid should contain", StringComparison.OrdinalIgnoreCase) ||
                        (fullContext.Contains("Guid", StringComparison.OrdinalIgnoreCase) &&
                         (fullContext.Contains("format", StringComparison.OrdinalIgnoreCase) || fullContext.Contains("convert", StringComparison.OrdinalIgnoreCase))))
                    {
                        translated = "El formato del identificador no es válido. Debe ser un GUID válido (ej: 00000000-0000-0000-0000-000000000000).";
                    }
                    // 2. String esperado (ej: número u objeto enviado en un campo string)
                    else if ((underlying == typeof(string) && isConversionError) ||
                             fullContext.Contains("could not be converted to System.String", StringComparison.OrdinalIgnoreCase))
                    {
                        translated = "El valor del campo debe ser una cadena de texto (string), no un número ni un objeto.";
                    }
                    // 3. Entero esperado
                    else if (((underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short)) && isConversionError) ||
                             fullContext.Contains("could not be converted to System.Int", StringComparison.OrdinalIgnoreCase) ||
                             fullContext.Contains("System.Int32", StringComparison.OrdinalIgnoreCase) ||
                             fullContext.Contains("System.Int64", StringComparison.OrdinalIgnoreCase))
                    {
                        translated = "El valor del campo debe ser un número entero válido.";
                    }
                    // 4. Booleano esperado
                    else if ((underlying == typeof(bool) && isConversionError) ||
                             fullContext.Contains("could not be converted to System.Boolean", StringComparison.OrdinalIgnoreCase))
                    {
                        translated = "El valor del campo debe ser un booleano (true o false).";
                    }
                    // 5. Fecha esperada
                    else if (((underlying == typeof(DateTime) || underlying == typeof(DateTimeOffset)) && isConversionError) ||
                             fullContext.Contains("could not be converted to System.DateTime", StringComparison.OrdinalIgnoreCase))
                    {
                        translated = "El formato de la fecha no es válido.";
                    }
                    // 6. JSON malformado / error de sintaxis JSON
                    else if (fullContext.Contains("JSON", StringComparison.OrdinalIgnoreCase) &&
                             (fullContext.Contains("syntax", StringComparison.OrdinalIgnoreCase) ||
                              fullContext.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
                              fullContext.Contains("expected", StringComparison.OrdinalIgnoreCase) ||
                              fullContext.Contains("end of data", StringComparison.OrdinalIgnoreCase) ||
                              (err.Exception is System.Text.Json.JsonException && fullContext.Contains("Path:"))))
                    {
                        translated = "El cuerpo de la solicitud contiene formato JSON inválido o mal estructurado.";
                    }
                    // 7. Request body vacío o 'The dto field is required'
                    else if (fullContext.Contains("non-empty request body", StringComparison.OrdinalIgnoreCase) ||
                             fullContext.Contains("The dto field is required", StringComparison.OrdinalIgnoreCase) ||
                             fullContext.Contains("The body field is required", StringComparison.OrdinalIgnoreCase) ||
                             (fullContext.Contains("field is required", StringComparison.OrdinalIgnoreCase) && isDtoRootKey))
                    {
                        translated = "El cuerpo de la solicitud no puede estar vacío. Debe enviar un JSON con los datos requeridos.";
                    }
                    // 8. Mensajes estándar de 'The {0} field is required' en inglés
                    else if (System.Text.RegularExpressions.Regex.IsMatch(msg, @"^The .+ field is required\.?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        translated = $"El campo '{displayKey}' es obligatorio y no puede ser nulo o vacío.";
                    }
                    // 9. Conversión genérica fallida
                    else if (fullContext.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
                    {
                        translated = "El tipo de dato proporcionado no es compatible con el campo esperado.";
                    }
                    // 10. Si ya es un mensaje en español de DataAnnotation o personalizado
                    else if (!string.IsNullOrWhiteSpace(msg))
                    {
                        translated = msg;
                    }
                    else
                    {
                        translated = "La solicitud contiene errores de formato o validación en los datos enviados.";
                    }

                    keyList.Add(translated);
                    allTranslated.Add(translated);
                }

                if (keyList.Count > 0)
                {
                    details[displayKey] = keyList.ToArray();
                }
            }

            // Priorizar un mensaje específico (ej: GUID inválido o tipo) sobre el mensaje genérico de DTO vacío
            var specificError = allTranslated.FirstOrDefault(e =>
                !e.Contains("cuerpo de la solicitud no puede estar vacío", StringComparison.OrdinalIgnoreCase) &&
                !e.Contains("The dto field is required", StringComparison.OrdinalIgnoreCase));

            var primaryMessage = specificError ?? allTranslated.FirstOrDefault()
                ?? "La solicitud contiene errores de formato o validación.";

            var responseObj = new
            {
                error = new
                {
                    code = "BadRequest",
                    message = primaryMessage,
                    details = details
                },
                message = primaryMessage
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
builder.Services.AddScoped<ITokenService, TokenService>();
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