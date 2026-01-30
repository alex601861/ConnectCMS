using CMSTrain.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace CMSTrain.Application.Settings;

public class DatabaseSettings : IValidatableObject
{
    public string DbProvider {  get; set; }

    public string? NpgSqlConnectionString { get; set; }

    public string? SqlServerConnectionString { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DbProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DbProvider)} is not configured.",
                new[] { nameof(DbProvider) });
        }

        if (string.IsNullOrEmpty(NpgSqlConnectionString) && string.IsNullOrEmpty(SqlServerConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}. A valid connection string for {GetDbProviderKey(DbProvider)} is not configured.");
        }
    }

    private static string GetDbProviderKey(string dbProvider)
    {
        return dbProvider switch
        {
            Constants.DbProviderKeys.Npgsql => "PostgreSQL",
            Constants.DbProviderKeys.SqlServer => "SQL Service",
            _ => ""
        };
    }
}
