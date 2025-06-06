using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories.Sqlite;

/// <summary>
/// Provides a SqliteConnection property that is automatically
/// created using the connection string, and disposed by the GC.
/// And also inits the tables on create.
/// </summary>
public class SqliteDataAccess : IAsyncDisposable, IDisposable
{
    private readonly SqliteConnection _sqlite;
    private bool _connected = false;

    private static readonly string[] Tables =
    {
        """
        create table if not exists invoices(
            id text primary key not null, 
            order_id text,
            status text not null,
            customer_id text not null
        );
        """,
        """
        create table if not exists products(
            id text primary key not null,
            name varchar(256) not null,
            short_description varchar(256) not null,
            long_description text not null,
            price double not null,
            listed bool not null
        );
        """,
        """
        create table if not exists orders(
            id text primary key not null,
            customer_id text not null,
            street text,
            city text,
            country text,
            post_code text,
            payment_provider text
        );
        """,
        """
        create table if not exists order_items(
            id text primary key not null,
            product_id text not null,
            order_id text not null,
            price_paid double not null,
            name varchar(256) not null,
            short_description varchar(256) not null,
            quantity_ordered int not null
        );
        """,
        """
        create table if not exists users(
            id text primary key not null,
            role_id int not null,
            first_name text not null,
            last_name text not null,
            password_hash text not null,
            email_address text not null,
            phone_number text not null
        );
        """,
    };

    public SqliteDataAccess(string connectionString)
    {
        _sqlite = new SqliteConnection(connectionString);
    }

    /// <summary>
    /// Returns a new command from the database. If the database is not open, it will connect to
    /// it before creating the command
    /// </summary>
    public async Task<SqliteCommand> CreateCommand()
    {
        await Connect();
        return _sqlite.CreateCommand();
    }

    public async Task InitTablesAsync()
    {
        var command = await CreateCommand();
        foreach (var query in Tables)
        {
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task Connect()
    {
        if (!_connected)
        {
            await _sqlite.OpenAsync();
            _connected = true;
        }
    }

    public void Dispose()
    {
        if (_connected) _sqlite.Close();
        _sqlite.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connected) await _sqlite.CloseAsync();
        await _sqlite.DisposeAsync();
    }
}