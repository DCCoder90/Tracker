using Tracker.Data.Repository;
using Tracker.RavenDb;
using Tracker.Redis;
using Tracker.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRepository>(new RavenRepository());
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();