using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;

namespace SkillBuilderPro.API.DrillImport;

public static class DrillImportCommand
{
    public static async Task<int> RunAsync(
        IServiceProvider services,
        IHostEnvironment environment,
        string[] args)
    {
        if (!environment.IsDevelopment())
        {
            Console.Error.WriteLine("The drill import command is restricted to the Development environment.");
            return 2;
        }

        var options = Parse(args);
        if (!options.TryGetValue("source", out var source)
            || !options.TryGetValue("sha256", out var expectedHash))
        {
            Console.Error.WriteLine(
                "Usage: import-drills --source <absolute-json-path> --sha256 <expected-sha256> [--dry-run]");
            return 2;
        }

        if (!Path.IsPathFullyQualified(source))
        {
            Console.Error.WriteLine("--source must be an absolute path.");
            return 2;
        }

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pending = (await db.Database.GetPendingMigrationsAsync()).ToArray();
        if (pending.Length > 0)
        {
            Console.Error.WriteLine(
                $"Import refused because migrations are pending: {string.Join(", ", pending)}");
            return 3;
        }

        var importer = scope.ServiceProvider.GetRequiredService<DrillImportService>();
        try
        {
            var result = await importer.RunAsync(
                source,
                expectedHash,
                options.ContainsKey("dry-run"));
            Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
            return result.Success ? 0 : 4;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Drill import failed and was rolled back: {exception.Message}");
            return 5;
        }
    }

    private static Dictionary<string, string> Parse(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            if (!argument.StartsWith("--", StringComparison.Ordinal)) continue;
            var key = argument[2..];
            if (string.Equals(key, "dry-run", StringComparison.OrdinalIgnoreCase))
            {
                options[key] = "true";
                continue;
            }

            if (index + 1 < args.Length && !args[index + 1].StartsWith("--", StringComparison.Ordinal))
                options[key] = args[++index];
        }
        return options;
    }
}
