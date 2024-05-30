using Tracker.Data.Repository;
using Tracker.RavenDb;
using Tracker.Service;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        WorkerOptions options = configuration.GetSection("Config").Get<WorkerOptions>();
        BackingOptions backingOptions = configuration.GetSection("Backing").Get<BackingOptions>();


        services.AddSingleton<BackingOptions>(backingOptions);
        services.AddSingleton(options);
        services.AddSingleton(typeof(IRepository),typeof(RavenRepository));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();