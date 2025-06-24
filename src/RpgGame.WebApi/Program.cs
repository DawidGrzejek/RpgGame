using System.Text.Json.Serialization;
using Asp.Versioning;
using RpgGame.Application; // For application layer DI extension
using RpgGame.Application.Events;
using RpgGame.Domain; // For domain layer DI extension
using RpgGame.Domain.Events.Characters;
using RpgGame.Infrastructure; // For infrastructure layer DI extension
using RpgGame.WebApi.Filters;
using RpgGame.WebApi.Hubs;
using RpgGame.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Register layers in the correct order (inner to outer)

// 1. Domain layer (doesn't depend on other layers)
builder.Services.AddDomainServices();

// 2. Application layer (depends on Domain)
builder.Services.AddApplicationServices();

// 3. Infrastructure layer (implements Application interfaces)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RPG Game API",
        Version = "v1",
        Description = "API for RPG Game"
    });
    c.CustomSchemaIds(type => type.FullName);
});

// Add to Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddApiVersioning()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.MapHub<GameHub>("/gameHub");

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();