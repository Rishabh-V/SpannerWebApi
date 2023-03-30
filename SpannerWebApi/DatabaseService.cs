using Google.Cloud.Spanner.Data;

namespace SpannerWebApi;

/// <summary>
/// This service runs in the background and ensures that the database is created upon application start.
/// </summary>
public class DatabaseService : BackgroundService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("SpannerConnection");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CreateDatabaseAndTableAsync();
        await InsertDataAsync();
    }

    // Keeping one long method for simplicity and mostly being lazy.
    private async Task CreateDatabaseAndTableAsync()
    {
        var builder = new SpannerConnectionStringBuilder(_connectionString);
        using SpannerConnection connection = new SpannerConnection(builder.ConnectionString);

        // Create Database.
        SpannerCommand createDbCmd = connection.CreateDdlCommand($"CREATE DATABASE `{builder.SpannerDatabase}`");
        try
        {
            await createDbCmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Created database {Database}", builder.DatabaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception {ex} occurred while creating database.", ex);
        }
        
        // Create Singer's table.
        SpannerCommand createTableCmd = connection.CreateDdlCommand(
            @"CREATE TABLE Singers (
            SingerId INT64 NOT NULL,
            FirstName STRING(1024),
            LastName STRING(1024),
            SingerInfo BYTES(MAX)
        ) PRIMARY KEY (SingerId)");

        try
        {
            await createTableCmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Created Singers table.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception {ex} occurred while creating table.", ex);
        }        
    }

    private async Task InsertDataAsync()
    {
        using SpannerConnection connection = new SpannerConnection(_connectionString);
        // Insert data.
        var batchCommand = connection.CreateBatchDmlCommand();
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (1, 'Alice', 'Henderson')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (2, 'Bruce', 'Allison')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (3, 'Sonu', 'Nigam')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (4, 'Arijit', 'Singh')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (5, 'Kishore', 'Kumar')");

        try
        {
            await batchCommand.ExecuteNonQueryAsync();
            _logger.LogInformation("Inserted data in the Singers table.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception {ex} occurred while inserting data.", ex);
        }
    }
}
