using Data;
using Web.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

// Регистрация репозиториев
builder.Services
    .AddSingleton<WareRepository>()
    .AddSingleton<OrderRepository>()
    .AddSingleton<ItemRepository>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Swagger UI и HealthChecks
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.Run();