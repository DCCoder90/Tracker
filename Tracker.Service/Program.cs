using Tracker.Data.Repository;
using Tracker.RavenDb;
using Tracker.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRepository>(new RavenRepository());
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();