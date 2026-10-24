using System.Globalization;
using MySqlConnector;

namespace LearnHub.Mcp;

/// <summary>Read-only MySQL access. Every query is parameterised and the session is set read-only.</summary>
public sealed class Db
{
    public const int MaxRows = 500;
    private readonly string _connectionString;

    public Db()
    {
        _connectionString = Environment.GetEnvironmentVariable("LEARNHUB_DB") ?? Build();
    }

    private static string Build()
    {
        static string Env(string k, string d) => Environment.GetEnvironmentVariable(k) ?? d;
        var b = new MySqlConnectionStringBuilder
        {
            Server = Env("LEARNHUB_DB_HOST", "127.0.0.1"),
            Port = uint.Parse(Env("LEARNHUB_DB_PORT", "3306"), CultureInfo.InvariantCulture),
            UserID = Env("LEARNHUB_DB_USER", "root"),
            Password = Environment.GetEnvironmentVariable("LEARNHUB_DB_PASSWORD") ?? "",
            Database = "learnhub",
        };
        return b.ConnectionString;
    }

    /// <summary>"Today" for the dataset; honours LEARNHUB_NOW like the API does.</summary>
    public static DateTime Now =>
        DateTime.TryParse(Environment.GetEnvironmentVariable("LEARNHUB_NOW"), CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var t) ? t : DateTime.UtcNow;

    public async Task<List<Dictionary<string, object?>>> QueryAsync(
        string sql, int limit = 100, params (string Name, object? Value)[] args)
    {
        limit = Math.Clamp(limit, 1, MaxRows);
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        await using (var ro = new MySqlCommand("SET SESSION TRANSACTION READ ONLY", conn))
            await ro.ExecuteNonQueryAsync();

        await using var cmd = new MySqlCommand(sql, conn);
        foreach (var (name, value) in args)
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);

        var rows = new List<Dictionary<string, object?>>();
        await using var r = await cmd.ExecuteReaderAsync();
        while (rows.Count < limit && await r.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < r.FieldCount; i++)
                row[r.GetName(i)] = r.IsDBNull(i) ? null : r.GetValue(i);
            rows.Add(row);
        }
        return rows;
    }
}
