
using Asp.Versioning;
using StreamingTitles.Api;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StreamingTitles.Api", Version = "v1" });
});

builder.Services.AddTransient<TitlesContext>();
builder.Services.AddTransient<ITitlesRepository, TitlesRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IPlatformRepository, PlatformRepository>();
var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var seed = scope.ServiceProvider.GetService<Seed>();
        seed.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    options.RoutePrefix = string.Empty; // Set the Swagger UI at the root URL
});
app.UseAuthorization();

app.MapControllers();

app.Run();