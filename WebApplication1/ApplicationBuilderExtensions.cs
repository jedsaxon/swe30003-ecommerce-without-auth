using DataAccess.Repositories.Sqlite;

namespace WebApplication1;

public static class ApplicationBuilderExtensions
{
    public static async Task InitTables(this IApplicationBuilder builder)
    {
        await using var scope = builder.ApplicationServices.CreateAsyncScope();

        var db = scope.ServiceProvider.GetService<SqliteDataAccess>();
        if (db is null) throw new NullReferenceException("You did not provide the SqliteDataAccess service");

        await db.InitTablesAsync();
    }
}