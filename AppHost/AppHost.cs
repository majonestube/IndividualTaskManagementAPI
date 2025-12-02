var builder = DistributedApplication.CreateBuilder(args);

var api = builder
    .AddProject<Projects.TaskManagementAPI>("Api");

builder
    .AddProject<Projects.Frontend>("Frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
