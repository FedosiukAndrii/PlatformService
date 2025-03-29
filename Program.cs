using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddScoped<HttpClient>();
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

        builder.Services.AddScoped<ICommandDataClient, HttpCommandDataClient>();
        builder.Services.AddSingleton<MessageBusClientWrapper>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<MessageBusClientWrapper>());
        builder.Services.AddSingleton(sp => sp.GetRequiredService<MessageBusClientWrapper>().Client);


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine("--> Using InMemoryDatabase");
                opt.UseInMemoryDatabase("Memory");
            }
            else
            {
                Console.WriteLine("--> Using SQL Server");
                Console.WriteLine(builder.Configuration.GetConnectionString("PlatformsConn"));
                opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
            }
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

        app.PrepPopulation(app.Environment.IsProduction());

        app.Run();
    }
}
