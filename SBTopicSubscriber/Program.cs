using Azure.Identity;
using DAL.DB;
using DAL.Repository;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    ;
var localConfiguration = configurationBuilder.Build();
var keyVaultUrl = localConfiguration["Azure:KeyVaultUrl"] ?? "";
var userAssignedClientId = localConfiguration["Azure:UserAssignedClientId"] ?? "";

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        /// Load the configuration from local.settings.json
        config.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
        //config.AddServiceBusClient(hostContext.Configuration["SBConnectionString"] ?? "");

        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            //InteractiveBrowserCredential
            config.AddAzureKeyVault(new Uri(keyVaultUrl), string.IsNullOrEmpty(userAssignedClientId) ? new DefaultAzureCredential() :
                new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId })
                );
            //config.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
            //var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            //config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
            //var secretResponse = secretClient.GetSecret("SBConnectionString");
            //var secretValue = secretResponse.Value.Value;
            ////Modify or add configuration values as needed

            //config.AddInMemoryCollection(new Dictionary<string, string>
            //{
            //    { "SBConnectionString", secretValue },
            //});
            //hostContext.Configuration["SBConnectionString"] = secretValue;
        }
    })
    //.ConfigureWebJobs(a => a.AddServiceBus())
    .ConfigureServices((hostContext, services) =>
    {
        //services.AddAzureClients(builder =>
        //{
        //    builder.AddSecretClient(hostContext.Configuration.GetSection("KeyVault"));
        //});
        //services.AddSingleton<SecretManagerService>();

        string connectionString = hostContext.Configuration["SqlConnectionString"] ?? "";
        //services.AddSingleton<IDbConnection>(provider => new SqlConnection(connectionString));
        services.AddSingleton(new DbConnectionFactory(connectionString));
        services.AddScoped<NotificationService>(); // Register the class directly without an interface
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }).Build();

host.Run();
