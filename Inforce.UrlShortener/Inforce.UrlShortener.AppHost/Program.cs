var builder = DistributedApplication.CreateBuilder(args);

const string databaseName = "UrlShortenerDb";
var postgres = builder
    .AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", databaseName)
    .WithDataVolume(isReadOnly: false);
var database = postgres.AddDatabase(databaseName);

var web = builder.AddProject<Projects.Inforce_UrlShortener_Web>("web")
    .WithReference(database)
    .WaitFor(database);

builder.AddNpmApp("angular", "../inforce-url-shortener-frontend")
    .WithReference(web)
    .WaitFor(web)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();