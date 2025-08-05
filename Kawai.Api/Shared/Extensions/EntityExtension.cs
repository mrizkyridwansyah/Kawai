using Microsoft.Data.SqlClient;
using Dapper;
using System.Text;

namespace Kawai.Api;

public static class EntityExtension
{
    public static void UseKawaiEntity(this WebApplication app)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var connectionString = configuration.GetConnectionString("Db");

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        string projectSqlPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "..", "..", "Kawai.Data", "Sql", "Db"));
        string runtimeSqlPath = Path.Combine(AppContext.BaseDirectory, "Sql", "Db");


        if (env.IsDevelopment())
        {
            Console.WriteLine("Running in Development mode. Using SQL from project directory.");
            ExecuteSqlSchemas(connection, projectSqlPath);
        }
        else
        {
            Console.WriteLine("Running in Production mode. Copying SQL files to runtime directory.");
            CopySqlFilesToRuntime(projectSqlPath, runtimeSqlPath);
            ExecuteSqlSchemas(connection, runtimeSqlPath);
        }
    }

    private static void ExecuteSqlSchemas(SqlConnection connection, string basePath)
    {
        CreateOrAlterSchema(connection, Path.Combine(basePath, "Views"));
        CreateOrAlterSchema(connection, Path.Combine(basePath, "Functions"));
        CreateOrAlterSchema(connection, Path.Combine(basePath, "StoreProcedures"));
        CreateOrAlterSchema(connection, Path.Combine(basePath, "Indexs"));
    }

    private static void CreateOrAlterSchema(SqlConnection connection, string path)
    {

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        var sqlFiles = Directory.GetFiles(path, "*.sql", SearchOption.TopDirectoryOnly);

        if (sqlFiles.Length == 0)
        {
            Console.WriteLine("No .sql files found to execute in: " + path);
            return;
        }

        foreach (var filePath in sqlFiles)
        {
            Console.WriteLine($"Executing SQL file: {Path.GetFileName(filePath)}");
            try
            {
                var sqlContent = File.ReadAllText(filePath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(sqlContent))
                {
                    Console.WriteLine($"Skipped empty SQL file: {Path.GetFileName(filePath)}");
                    continue;
                }

                connection.Execute(sqlContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing {filePath}: {ex.Message}");
            }
        }
    }


    private static void CopySqlFilesToRuntime(string sourceRoot, string targetRoot)
    {
        if (!Directory.Exists(sourceRoot)) return;

        foreach (var dir in new[] { "Views", "Functions", "StoreProcedures", "Indexs" })
        {
            var sourceDir = Path.Combine(sourceRoot, dir);
            var targetDir = Path.Combine(targetRoot, dir);

            if (!Directory.Exists(sourceDir)) continue;

            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir, "*.sql", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);

                File.Copy(file, destFile, overwrite: true);
            }
        }
    }
}
