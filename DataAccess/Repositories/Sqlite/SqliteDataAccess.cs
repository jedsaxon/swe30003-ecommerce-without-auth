using Microsoft.Data.Sqlite;

namespace DataAccess.Repositories.Sqlite;

/// <summary>
/// Provides a SqliteConnection property that is automatically
/// created using the connection string, and disposed by the GC.
/// And also inits the tables on create.
/// </summary>
public class SqliteDataAccess : IAsyncDisposable, IDisposable
{
    public SqliteConnection Sqlite { get; private set; }

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
            price double not null
        );
        """,
        """
        create table if not exists orders(
            id text primary key not null,
            customer_id text not null
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
            role text not null,
            first_name text not null,
            last_name text not null,
            email_address text not null,
            phone_number text not null,
            address_street text not null,
            address_city text not null,
            address_postal_code text not null,
            address_country text not null
        );
        """,
    };

    public SqliteDataAccess(string? connectionString)
    {
        Sqlite = new SqliteConnection(connectionString);
        Sqlite.Open();
    }

    public async Task InitTablesAsync()
    {
        var command = Sqlite.CreateCommand();
        foreach (var query in Tables)
        {
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }
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