using Microsoft.Data.Sqlite;

namespace DataAccess;

public class DataAccess : IAsyncDisposable, IDisposable
{
    private readonly SqliteConnection _connection;

    public DataAccess(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}