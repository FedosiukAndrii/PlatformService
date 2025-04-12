using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.BackgroundServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddScoped<HttpClient>();
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

        builder.Services.AddScoped<ICommandDataClient, HttpCommandDataClient>();
        builder.Services.AddSingleton<IMessageBusClient>(await MessageBusClient.CreateAsync(builder.Configuration));
        builder.Services.AddHostedService<OutboxProcessorService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddGrpc();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            Console.WriteLine("--> Using SQL Server");
            Console.WriteLine(builder.Configuration.GetConnectionString("PlatformsConn"));
            opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
        });
        Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandServiceApiUrl"]}");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapGrpcService<GrpcPlatformService>();
        app.MapGet("/protos/platforms.proto", async context => await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto")));

        app.PrepPopulation();

        app.Run();
    }
}
