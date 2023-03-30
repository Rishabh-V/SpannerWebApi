using Google.Api.Gax;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using Microsoft.AspNetCore.Mvc;

namespace SpannerWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(IConfiguration configuration, ILogger<SessionsController> logger)
    {
        _connectionString = configuration.GetConnectionString("SpannerConnection");
        _logger = logger;
    }

    // GET: api/<SessionsController>
    [HttpGet]
    public async IAsyncEnumerable<Session> GetAsync()
    {
        var builder = new SpannerConnectionStringBuilder(_connectionString);
        using var connection = new SpannerConnection(builder.ConnectionString);
        connection.Open();

        var client = connection.GetSpannerClient();
        ListSessionsRequest request = new ListSessionsRequest
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(builder.Project, builder.SpannerInstance, builder.SpannerDatabase)
        };

        PagedAsyncEnumerable<ListSessionsResponse, Session> sessions = client.ListSessionsAsync(request);
        int count = 0;
        await foreach(Session session in sessions)
        {
            count += 1;
            _logger.LogInformation($"Total sessions {count} so far. {session}" );
            yield return session;
        }
    }

    // GET: api/<SessionsController>/ids
    [HttpGet("ids")]
    public async IAsyncEnumerable<string> GetSessionIdsAsync()
    {
        var builder = new SpannerConnectionStringBuilder(_connectionString);
        using var connection = new SpannerConnection(builder.ConnectionString);
        connection.Open();

        var client = connection.GetSpannerClient();
        ListSessionsRequest request = new ListSessionsRequest
        {
            DatabaseAsDatabaseName = DatabaseName.FromProjectInstanceDatabase(builder.Project, builder.SpannerInstance, builder.SpannerDatabase)
        };

        var sessions = client.ListSessionsAsync(request);
        int count = 0;
        await foreach (Session session in sessions)
        {
            count += 1;
            _logger.LogInformation($"Total sessions {count} so far. {session}");
            yield return session.SessionName.SessionId;
        }
    }

    [HttpDelete]
    public void Delete(params string[] sessionIds)
    {
        if (sessionIds == null || !sessionIds.Any())
        {
            _logger.LogWarning("sessionIds is null or empty.");
            return;
        }

        var builder = new SpannerConnectionStringBuilder(_connectionString);
        using var connection = new SpannerConnection(builder.ConnectionString);
        connection.Open();

        var client = connection.GetSpannerClient();
        int count = 0;
        foreach (var sessionId in sessionIds)
        {
            DeleteSessionRequest request = new DeleteSessionRequest
            {
                SessionName = SessionName.FromProjectInstanceDatabaseSession(builder.Project, builder.SpannerInstance, builder.SpannerDatabase, sessionId)
            };

            client.DeleteSession(request);
            count += 1;
            _logger.LogInformation($"Deleted session {sessionId}. Total Deleted {count}");
        }
    }
}
