using CMSTrain.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CMSTrain.Helper;

public static class DatabaseHelper
{
    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            Constants.DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                e.MigrationsAssembly("CMSTrain.Migrators.PostgreSQL")),
            Constants.DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString, e =>
                e.MigrationsAssembly("CMSTrain.Migrators.SQLServer")),
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported."),
        };
    }
}