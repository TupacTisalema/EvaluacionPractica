using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OrderProcessingWorkerService;
using OrderProcessingWorkerService.Config;
using OrderProcessingWorkerService.Interfaces;
using OrderProcessingWorkerService.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IOrderProcessor, OrderProcessor>(); // Mantener como Scoped
        services.AddScoped<IOrderRepository, OrderRepository>(); // Mantener como Scoped
        services.AddScoped<IErrorLogger, ErrorLogger>(); // Mantener como Scoped
        services.AddScoped<IReportGenerator, ReportGenerator>(); // Mantener como Scoped

        // Configuración adicional para la expresión Cron y los parámetros de concurrencia
        services.Configure<WorkerOptions>(hostContext.Configuration.GetSection("WorkerOptions"));
    });

var host = builder.Build();
host.Run();