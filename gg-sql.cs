#!/usr/bin/env -S dotnet --
#:package GridGain.Ignite@9.1.23
using Apache.Ignite;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: gg-sql <script.sql>");
    return;
}

string endpoint = Environment.GetEnvironmentVariable("GG_ENDPOINT") ?? "localhost";
using var ignite = await IgniteClient.StartAsync(new IgniteClientConfiguration(endpoint));

await using var rs = await ignite.Sql.ExecuteAsync(null, args[0]);

if (rs.WasApplied)
{
    Console.WriteLine("Query executed successfully.");
}

if (rs.AffectedRows >= 0)
{
    Console.WriteLine($"Query executed successfully. Affected rows: {rs.AffectedRows}");
}

if (rs.HasRowSet)
{
    Console.WriteLine(string.Join(" \t ", rs.Metadata.Columns.Select(c => c.Name)));

    await foreach (var row in rs)
    {
        Console.WriteLine(string.Join(" \t ", Enumerable.Range(0, rs.Metadata.Columns.Count).Select(i => row[i])));
    }
}