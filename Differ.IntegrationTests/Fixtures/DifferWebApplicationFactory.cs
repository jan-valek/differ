using Differ.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Differ.IntegrationTests.Fixtures;

/// <summary>
/// Factory create a pre-configured test host environment for integration tests.
/// </summary>
internal class DifferWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public DifferWebApplicationFactory(string connectionString = "")
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DiffContext>));

            services.AddDbContext<DiffContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });

        builder.ConfigureTestServices(services =>
        {
            
            var sp = services.BuildServiceProvider();
            
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<DiffContext>();
            
            db.Database.Migrate();
        });
    }
}