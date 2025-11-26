using CommunityToolkit.Aspire.Hosting; 

var builder = DistributedApplication.CreateBuilder(args);

// --- 1. STORAGE (MINIO) ---
var storage = builder.AddMinioContainer("storage")
                     .WithDataVolume("minio-data");


// --- 2. CACHE (REDIS) ---
var redis = builder.AddRedis("cache")
                   .WithDataVolume("redis-data");

// --- 3. DATABASE (POSTGRES) ---
var postgresServer = builder.AddPostgres("postgres")
                            .WithLifetime(ContainerLifetime.Persistent)
                            .WithDataVolume("pg-data");

var postgresDB = postgresServer.AddDatabase("PostgresConnection");

// --- 4. BACKEND (API) ---
var apiService = builder.AddProject<Projects.CinePass_ApiService>("apiservice")
    .WithReference(redis)
    .WithReference(postgresDB)
    .WithReference(storage)
    .WithHttpsEndpoint(port: 5001, name: "my-https")
    .WithHttpEndpoint(port: 5000, name: "my-http")
    .WithExternalHttpEndpoints();

// --- 5. FRONTEND (ADMIN) ---
builder.AddNpmApp("admin-web", "../cinema-admin", "dev")
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT", port: 8080, name: "endpoint")
    .WithExternalHttpEndpoints();

builder.Build().Run();