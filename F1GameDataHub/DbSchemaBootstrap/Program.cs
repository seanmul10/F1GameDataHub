using Npgsql;

var connString =
    Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
    ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

var bootstrapProjectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
var schemaFilesPath = Path.Combine(bootstrapProjectPath, "SchemaFiles");
var scriptsSchemaPath = Path.GetFullPath(Path.Combine(bootstrapProjectPath, "..", "..", "Scripts", "Schema"));

var sqlFiles = new List<string>();

if (Directory.Exists(schemaFilesPath))
{
    var bootstrapOrder = new[]
    {
        "CreateSessionTable.sql",      // Creates session_metadata
        "CreateParticipantsTable.sql", // Depends on session_metadata
        "CreateTelemetryTable.sql"     // Creates car_telemetry
    };

    foreach (var fileName in bootstrapOrder)
    {
        var fullPath = Path.Combine(schemaFilesPath, fileName);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Expected schema file not found: {fullPath}");
        }

        sqlFiles.Add(fullPath);
    }
}

if (Directory.Exists(scriptsSchemaPath))
{
    sqlFiles.AddRange(
        Directory.GetFiles(scriptsSchemaPath, "*.sql", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file)
    );
}

if (sqlFiles.Count == 0)
{
    throw new DirectoryNotFoundException(
        $"No schema files found. Checked: {schemaFilesPath} and {scriptsSchemaPath}"
    );
}

foreach (var file in sqlFiles)
{
    Console.WriteLine($"Executing SQL file: {file}");

    var sql = await File.ReadAllTextAsync(file);
    await using var cmd = new NpgsqlCommand(sql, conn);
    var result = await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"Executed {result} rows from {file}");
}