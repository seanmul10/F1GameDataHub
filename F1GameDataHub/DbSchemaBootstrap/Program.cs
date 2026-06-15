using Npgsql;

// Connection string to your TimescaleDB/PostgreSQL instance
var connString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

var sqlFiles = Directory.GetFiles("SchemaFiles", "*.sql");

foreach (var file in sqlFiles)
{
    Console.WriteLine($"Executing SQL file: {file}");

    var sql = await File.ReadAllTextAsync(file);
    await using var cmd = new NpgsqlCommand(sql, conn);
    var result = await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"Executed {result} rows from {file}");
}