using Testcontainers.MsSql;

namespace Differ.IntegrationTests.Fixtures;

/// <summary>
/// Customize webapi program to be able to connect to the temporary sql server instance.
/// </summary>
public class TestFixture : IAsyncLifetime
{
    private DifferWebApplicationFactory? _factory;
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Test@123456")
        .Build();

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        
        var connectionString = _msSqlContainer.GetConnectionString(); // contains masterdb as default.
        connectionString = connectionString.Replace("Database=master", "Database=DifferTestDb");
        
        _factory = new DifferWebApplicationFactory(connectionString);
        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
        await _msSqlContainer.DisposeAsync();
    }
}