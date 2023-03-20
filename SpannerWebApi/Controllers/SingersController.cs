using Google.Cloud.Spanner.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SpannerWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SingersController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ILogger<SingersController> _logger;

    public SingersController(IConfiguration configuration, ILogger<SingersController> logger)
    {
        _connectionString = configuration.GetConnectionString("SpannerConnection");
        _logger = logger;
    }

    // GET: api/<SingersController>
    [HttpGet]
    public async IAsyncEnumerable<Singer> GetAsync()
    {
        using var connection = new SpannerConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateSelectCommand("SELECT * FROM Singers");
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var singer = new Singer
            {
                Id = reader.GetFieldValue<long>("SingerId"),
                FirstName = reader.GetFieldValue<string>("FirstName"),
                LastName = reader.GetFieldValue<string>("LastName")
            };

            _logger.LogInformation("SingerId :{Id}, FirstName: {FirstName}, LastName: {LastName}", singer.Id, singer.FirstName, singer.LastName); 
            yield return singer;
        }
    }

    // GET api/<SingersController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Singer>> GetByIdAsync(long id)
    {
        using var connection = new SpannerConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateSelectCommand("SELECT * FROM Singers WHERE SingerId=@id", new SpannerParameterCollection { { "id", SpannerDbType.Int64, id } });
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var singer = new Singer
            {
                Id = reader.GetFieldValue<long>("SingerId"),
                FirstName = reader.GetFieldValue<string>("FirstName"),
                LastName = reader.GetFieldValue<string>("LastName")
            };

            _logger.LogInformation("SingerId :{Id}, FirstName: {FirstName}, LastName: {LastName}", singer.Id, singer.FirstName, singer.LastName);
            return singer;
        }
        else
        {
            return NotFound();
        }
    }

    // POST api/<SingersController>
    [HttpPost]
    public async Task<ActionResult<Singer>> PostAsync([FromBody] Singer value)
    {
        using var connection = new SpannerConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateInsertCommand("Singers", new SpannerParameterCollection
                {
                    {"SingerId", SpannerDbType.Int64, value.Id },
                    {"FirstName", SpannerDbType.String, value.FirstName },
                    {"LastName", SpannerDbType.String, value.LastName }
                });
        await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Created Singer with ID {ID}", value.Id);
        return await GetByIdAsync(value.Id);
    }

    // PUT api/<SingersController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> PutAsync(long id, [FromBody] Singer value)
    {
        using var connection = new SpannerConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateUpdateCommand("Singers", new SpannerParameterCollection
                {
                    {"SingerId", SpannerDbType.Int64, value.Id },
                    {"FirstName", SpannerDbType.String, value.FirstName },
                    {"LastName", SpannerDbType.String, value.LastName }
                });
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Rows {Rows} updated while updating Singer with ID {ID}", rowsAffected, value.Id);
        return rowsAffected == 0 ? NotFound() : NoContent();
    }

    // DELETE api/<SingersController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        using var connection = new SpannerConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateDeleteCommand("Singers", new SpannerParameterCollection
        {
            {"SingerId", SpannerDbType.Int64, id }
        });
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        _logger.LogInformation("Rows {Rows} deleted while deleting Singer with ID {ID}", rowsAffected, id);
        return rowsAffected == 0 ? NotFound() : NoContent();
    }
}
