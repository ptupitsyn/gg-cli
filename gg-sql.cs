#!/usr/bin/env -S dotnet --

#:package GridGain.Ignite@9.1.23
#:package Spectre.Console@0.57.0

using Apache.Ignite;
using Spectre.Console;

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
    var table = new Table()
        .RoundedBorder()
        .AddColumns(rs.Metadata.Columns.Select(c => c.Name).ToArray());

    await foreach (var row in rs)
    {
        table.AddRow(Enumerable.Range(0, row.FieldCount).Select(i => row[i]?.ToString() ?? "").ToArray());
    }

    AnsiConsole.Write(table);
}