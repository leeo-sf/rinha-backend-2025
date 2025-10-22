using Microsoft.EntityFrameworkCore;
using Refit;
using RinhaBackend.Api.Application.Contract;
using RinhaBackend.Api.Application.Factory;
using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Application.Service;
using RinhaBackend.Api.Application.Service.Api;
using RinhaBackend.Api.Data;
using RinhaBackend.Api.Data.Repository;
using RinhaBackend.Api.Domain.Interface;
using RinhaBackend.Api.Presentation.Endpoints;
using RinhaBackend.Api.Presentation.Worker;
using System.Threading.Channels;

namespace RinhaBackend.Api.Infra;

public static class AppDependenciesConfiguration
{
    public static void ConfigKestrel(this WebApplicationBuilder webHost)
        => webHost.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxConcurrentConnections = int.MaxValue;
            options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(30);
        });

    public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        => services.AddDbContextPool<AppDbContext>(opt =>
        {
            opt.UseNpgsql(configuration["ConnectionStrings:Database"]);
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
        services.AddSingleton<IPaymentProcessor, DefaultPaymentProcessor>();
        services.AddSingleton<IPaymentProcessor, FallbackPaymentProcessor>();
        services.AddSingleton<PaymentProcessorFactory>();
        services.AddSingleton(Channel.CreateUnboundedPrioritized<PaymentContract>());
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddSingleton<IPaymentQueueService, PaymentQueueService>();
        services.AddMemoryCache();
    }

    public static void ConfigureWorkerServices(this IServiceCollection services)
    {
        services.AddHostedService<ProcessPaymentWorkerService>();
        services.AddHostedService<PaymentHealthCheckWorkerService>();
    }

    public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddPaymentEndpoints();
        return app;
    }

    private static void ConfigureRefitClient<T>(this IServiceCollection services, string host)
        where T : class =>
        services
            .AddRefitClient<T>()
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = new Uri(host);
            });
}