using Eras.Infrastructure.Persistence.PostgreSQL.Migrations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Migrations;

public sealed class BackfillEvaluationPollsMigrationTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public BackfillEvaluationPollsMigrationTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        Execute("PRAGMA foreign_keys = ON;");
        Execute(@"
            CREATE TABLE evaluation (""Id"" INTEGER PRIMARY KEY);
            CREATE TABLE polls (""Id"" INTEGER PRIMARY KEY, uuid TEXT NOT NULL);
            CREATE TABLE poll_instances (""Id"" INTEGER PRIMARY KEY, uuid TEXT NOT NULL, ""EvaluationId"" INTEGER NULL);
            CREATE TABLE evaluation_poll (
                evaluation_id INTEGER NOT NULL REFERENCES evaluation (""Id""),
                poll_id INTEGER NOT NULL REFERENCES polls (""Id""),
                PRIMARY KEY (evaluation_id, poll_id));");
    }

    [Fact]
    public void Up_LinksEachPollWithInstancesToItsEvaluation()
    {
        Execute(@"
            INSERT INTO evaluation (""Id"") VALUES (1);
            INSERT INTO polls (""Id"", uuid) VALUES (10, 'a'), (11, 'b');
            INSERT INTO poll_instances (""Id"", uuid, ""EvaluationId"") VALUES (1, 'a', 1), (2, 'a', 1), (3, 'b', 1);");

        RunMigration();

        Assert.Equal(new[] { "1-10", "1-11" }, Links());
    }

    [Fact]
    public void Up_SkipsInstancesOfEvaluationsThatNoLongerExistInsteadOfFailing()
    {
        Execute(@"
            INSERT INTO evaluation (""Id"") VALUES (1);
            INSERT INTO polls (""Id"", uuid) VALUES (10, 'a'), (11, 'b');
            INSERT INTO poll_instances (""Id"", uuid, ""EvaluationId"") VALUES (1, 'a', 1), (2, 'b', 46), (3, 'a', 45);");

        RunMigration();

        Assert.Equal(new[] { "1-10" }, Links());
    }

    [Fact]
    public void Up_IgnoresInstancesWithoutEvaluation()
    {
        Execute(@"
            INSERT INTO evaluation (""Id"") VALUES (1);
            INSERT INTO polls (""Id"", uuid) VALUES (10, 'a');
            INSERT INTO poll_instances (""Id"", uuid, ""EvaluationId"") VALUES (1, 'a', NULL);");

        RunMigration();

        Assert.Empty(Links());
    }

    [Fact]
    public void Up_KeepsExistingLinksAndCanRunTwice()
    {
        Execute(@"
            INSERT INTO evaluation (""Id"") VALUES (1);
            INSERT INTO polls (""Id"", uuid) VALUES (10, 'a'), (11, 'b');
            INSERT INTO poll_instances (""Id"", uuid, ""EvaluationId"") VALUES (1, 'a', 1), (2, 'b', 1);
            INSERT INTO evaluation_poll (evaluation_id, poll_id) VALUES (1, 10);");

        RunMigration();
        RunMigration();

        Assert.Equal(new[] { "1-10", "1-11" }, Links());
    }

    public void Dispose() => _connection.Dispose();

    private void RunMigration()
    {
        foreach (SqlOperation operation in new BackfillEvaluationPolls().UpOperations.OfType<SqlOperation>())
        {
            Execute(operation.Sql);
        }
    }

    private List<string> Links()
    {
        using SqliteCommand command = _connection.CreateCommand();
        command.CommandText = "SELECT evaluation_id || '-' || poll_id FROM evaluation_poll ORDER BY evaluation_id, poll_id;";
        using SqliteDataReader reader = command.ExecuteReader();

        var links = new List<string>();
        while (reader.Read())
        {
            links.Add(reader.GetString(0));
        }

        return links;
    }

    private void Execute(string sql)
    {
        using SqliteCommand command = _connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
