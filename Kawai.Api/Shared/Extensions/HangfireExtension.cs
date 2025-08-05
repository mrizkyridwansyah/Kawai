using Hangfire;
using Microsoft.Data.SqlClient;

namespace Kawai.Api;

public static class HangfireExtension
{
    public static void AddKawaiHangfire(this IServiceCollection services)
    {
        services.AddHangfire(p => p.SetDataCompatibilityLevel(CompatibilityLevel.Version_170));
        services.AddHangfireServer();
    }
    public static void UseKawaiHangfire(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var config = scope.ServiceProvider.GetService<IConfiguration>();

        #region Hangfire
        // Ambil connection string asli
        var originalCnnStr = config.GetConnectionString("Db");

        // Ambil nama database dari connection string
        var builder = new SqlConnectionStringBuilder(originalCnnStr);
        var originalDbName = builder.InitialCatalog;

        // Buat nama DB Hangfire
        var hfDbName = $"{originalDbName}.Hangfire";

        // Buat connection string baru untuk Hangfire DB
        builder.InitialCatalog = hfDbName;
        var hangfireCnnStr = builder.ConnectionString;

        // Buat database Hangfire jika belum ada
        using (var masterConnection = new SqlConnection(originalCnnStr.Replace(originalDbName, "master")))
        {
            masterConnection.Open();

            var cmd = masterConnection.CreateCommand();
            cmd.CommandText = $@"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{hfDbName}')
                BEGIN
                    CREATE DATABASE [{hfDbName}]
                END";
            cmd.ExecuteNonQuery();
        }

        GlobalConfiguration.Configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(hangfireCnnStr);

        #endregion

        app.UseHangfireDashboard();
    }
}
