using Microsoft.EntityFrameworkCore;
using Refit;
using RinhaBackend.Api.Application.Config;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Service;
using RinhaBackend.Api.Application.Service.Api;
using RinhaBackend.Api.Data;
using RinhaBackend.Api.Data.Repository;
using RinhaBackend.Api.Domain.Handler;
using RinhaBackend.Api.Domain.Interface;
using RinhaBackend.Api.Worker;

namespace RinhaBackend.Api.Infra;

public static class AppDependenciesConfiguration
{
    public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(configuration["ConnectionStrings:Database"]);
        });

    public static void AddMediatorConfiguration(this IServiceCollection services)
        => services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblies(typeof(PaymentsHandler).Assembly);
        });

    public static void ConfigureAppDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureRefitClient<IPaymentProcessorDefaultApi>(configuration["Service:PaymentProcessorDefault:BASE_URL"]!);
        services.ConfigureRefitClient<IPaymentProcessorFallbackApi>(configuration["Service:PaymentProcessorFallback:BASE_URL"]!);
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureServiceCredentials<AppConfiguration>("Service", configuration);

        services.AddSingleton<IPaymentProcessorDefaultApiService, PaymentProcessorDefaultApiService>();
        services.AddSingleton<IPaymentProcessorFallbackApiService, PaymentProcessorFallbackApiService>();
        services.AddSingleton<IRabbitMQService, RabbitMQService>();
        services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddMemoryCache();
    }

    public static void ConfigureWorkerServices(this IServiceCollection services)
    {
        services.AddHostedService<ProcessPaymentWorkerService>();
        services.AddHostedService<PaymentProcessedWorkerService>();
        services.AddHostedService<PaymentDefaultHealthCheckWorkerService>();
        services.AddHostedService<PaymentFallbackHealthCheckWorkerService>();
    }

    private static void ConfigureRefitClient<T>(this IServiceCollection services, string host)
        where T : class =>
        services
            .AddRefitClient<T>()
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = new Uri(host);
            });

    private static void ConfigureServiceCredentials<T>(this IServiceCollection services, string sectionName, IConfiguration configuration) where T : class
        => services.AddSingleton(opt => configuration.GetSection(sectionName).Get<T>()!);
}