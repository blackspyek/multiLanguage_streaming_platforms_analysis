
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using StreamingTitles.Api;
using StreamingTitles.Data.Helper;
using StreamingTitles.Data.Model;
using StreamingTitles.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// migrate database at runtime



builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://front:3000", "http://web:5000", "http://localhost:3000", "http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StreamingTitles.Api", Version = "v1" });
});
builder.Services.AddSignalR();
builder.Services.AddSingleton<ILastModificationService, LastModificationService>();

builder.Services.AddTransient<TitlesContext>();
builder.Services.AddTransient<ITitlesRepository, TitlesRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IPlatformRepository, PlatformRepository>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TitlesContext>();
    context.Database.Migrate();
}

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var seed = scope.ServiceProvider.GetService<Seed>();
        seed.SeedDataContext();
        seed.SeedDataFromXML("../StreamingTitles.Data/Data/netflix_titles.xml");
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
app.UseCors();
app.MapControllers();
app.MapHub<ProgressHub>("/progressHub");

app.Run();
