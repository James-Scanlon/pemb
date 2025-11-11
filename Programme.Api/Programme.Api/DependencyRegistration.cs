using Audit.ApiClient;
using Microsoft.Extensions.DependencyInjection;
using Pemberton.ApiClient;
using Pemberton.KeyVault;
using Pemberton.Rates.Api.Client;
using Programme.Api.Domain;
using Programme.Api.Repositories;
using Wcf.Accounts.ApiClient;

namespace Programme.Api;

public static class DependencyRegistration
{
    public static void AddDependencies(this IServiceCollection serviceCollection)
    {
         serviceCollection.AddSingleton<IDatabaseConnection, DatabaseConnection>();
         serviceCollection.AddSingleton<IDatabaseConfiguration, DatabaseConfiguration>();

         serviceCollection.AddSingleton<ICurrencyRepository, CurrencyRepository>();
        serviceCollection.AddTransient<IProgrammeRepository, ProgrammeRepository>();
        serviceCollection.AddTransient<INextIdRepository, NextIdRepository>();
        serviceCollection.AddTransient<IKeyVaultService, KeyVaultService>();
       
        serviceCollection.AddTransient<IClientSecurity, ClientSecurity>();
        serviceCollection.AddSingleton<IAccountsApiConfig, AccountsApiConfig>();
        serviceCollection.AddScoped<IAccountsApiClient, AccountsApiClient>();

        serviceCollection.AddScoped<IRatesApiClient, RatesApiClient>();
        serviceCollection.AddSingleton<IRatesApiConfig, RatesApiConfig>();

        serviceCollection.AddSingleton<IAuditApiConfig, AuditApiConfig>();
        serviceCollection.AddScoped<IAuditApiClient, AuditApiClient>();

    }
}