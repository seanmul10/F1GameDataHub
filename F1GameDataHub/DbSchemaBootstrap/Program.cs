using Npgsql;

var connString =
    Environment.GetEnvironmentVariable("F1_DB_CONNECTION_STRING")
    ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

var bootstrapProjectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
var schemaFilesPath = Path.Combine(bootstrapProjectPath, "SchemaFiles");
var scriptsPath = Path.GetFullPath(Path.Combine(bootstrapProjectPath, "..", "..", "Scripts"));
var schemaTablesPath = Path.Combine(scriptsPath, "Schema", "Tables");
var staticDataPath = Path.Combine(scriptsPath, "StaticData");
var schemaViewsPath = Path.Combine(scriptsPath, "Schema", "Views");

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

if (!Directory.Exists(schemaTablesPath))
{
    throw new DirectoryNotFoundException($"Schema tables directory not found at: {schemaTablesPath}");
}

if (!Directory.Exists(staticDataPath))
{
    throw new DirectoryNotFoundException($"Static data directory not found at: {staticDataPath}");
}

if (!Directory.Exists(schemaViewsPath))
{
    throw new DirectoryNotFoundException($"Schema views directory not found at: {schemaViewsPath}");
}

sqlFiles.AddRange(Directory.GetFiles(schemaTablesPath, "*.sql").OrderBy(file => file));
sqlFiles.AddRange(Directory.GetFiles(staticDataPath, "*.sql").OrderBy(file => file));
sqlFiles.AddRange(Directory.GetFiles(schemaViewsPath, "*.sql").OrderBy(file => file));

if (sqlFiles.Count == 0)
{
    throw new DirectoryNotFoundException(
        $"No schema files found. Checked: {schemaFilesPath}, {schemaTablesPath}, {staticDataPath}, and {schemaViewsPath}"
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