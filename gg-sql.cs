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

await ignite.Sql.ExecuteScriptAsync(args[0]);