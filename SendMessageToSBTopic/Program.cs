using Azure.Identity;
using DAL.DB;
using DAL.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.ServiceBus;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

var localConfiguration = configurationBuilder.Build();
var keyVaultUrl = localConfiguration["Azure:KeyVaultUrl"];

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
        // Add Azure Key Vault secrets to configuration
        if (!string.IsNullOrEmpty(keyVaultUrl))
            config.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Other service registrations
        string connectionString = hostContext.Configuration["SqlConnectionString"] ?? "";
        //services.AddSingleton<IDbConnection>(provider => new SqlConnection(connectionString));
        services.AddSingleton(new ServiceBusMessageSender(hostContext.Configuration));
        services.AddSingleton(new DbConnectionFactory(connectionString));
        services.AddScoped<NotificationService>(); // Register the class directly without an interface
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }).Build();

host.Run();

