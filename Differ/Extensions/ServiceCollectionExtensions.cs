using Differ.CustomBinders;
using Differ.CustomBinders.Deserializer;
using Differ.Data;
using Differ.Repositories;
using Differ.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Differ.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCustomContentTypeModelBinder(this IServiceCollection services)
    {
        services.AddSingleton<CustomContentTypeModelBinderProvider>();
        services.AddHttpContextAccessor();
        services.AddSingleton<CustomContentTypeBinder>();
        services.AddSingleton<IBase64JsonDeserializer, Base64JsonToObjectDeserializer>();
        services.Configure<MvcOptions>(opt => opt.ModelBinderProviders.Add(new CustomContentTypeModelBinderProvider()));
        return services;
    }

    public static IServiceCollection RegisterStringComparerService(this IServiceCollection services)
    {
        services.AddScoped<IDiffRepository, DiffRepository>();
        services.AddScoped<IDiffStringComparer, DiffStringComparer>();
        return services;
    }

    public static IServiceCollection RegisterDb(this IServiceCollection services,IConfigurationManager configurationManager)
    {
        services.AddDbContext<DiffContext>(options =>
        {
            var connectionString = configurationManager.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        });
        return services;
    }

    public static WebApplication RunMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DiffContext>();
        dbContext.Database.Migrate();
        return app;
    } 
}