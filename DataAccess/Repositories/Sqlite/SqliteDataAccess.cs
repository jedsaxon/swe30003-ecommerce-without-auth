using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories.Sqlite;

/// <summary>
/// Provides a SqliteConnection property that is automatically
/// created using the connection string, and disposed by the GC.
/// </summary>
public class SqliteDataAccess : IAsyncDisposable, IDisposable
{
    public SqliteConnection Sqlite { get; private set; }

    public SqliteDataAccess(string connectionString)
    {
        Sqlite = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        Sqlite.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Sqlite.DisposeAsync();
    }
}