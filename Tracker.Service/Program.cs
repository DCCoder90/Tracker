using Tracker.Data.Repository;
using Tracker.RavenDb;
using Tracker.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        WorkerOptions options = configuration.GetSection("Config").Get<WorkerOptions>();

        services.AddSingleton(options);
        services.AddSingleton<IRepository>(new RavenRepository());
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();