using DAL.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using System.Data;
using System.Data.SqlClient;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        // Load the configuration from local.settings.json
        config.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Read the connection string from configuration and add IDbConnection as a service
        string connectionString = hostContext.Configuration.GetConnectionString("SqlConnectionString");
        services.AddSingleton<IDbConnection>(provider => new SqlConnection(connectionString));
        services.AddScoped<NotificationService>(); // Register the class directly without an interface
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }).Build();

host.Run();