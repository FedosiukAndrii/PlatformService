using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient(HttpClient httpClient, IConfiguration config) : ICommandDataClient
{
    public async Task SendPlatformToCommand(PlatformReadDTO platform)
    {
        var response = await httpClient.PostAsJsonAsync($"{config["CommandServiceApiUrl"]}/commands/platforms", platform);

        Console.WriteLine($"{response.RequestMessage.Method} {response.RequestMessage.RequestUri}");

        if (response.IsSuccessStatusCode)
            Console.WriteLine("--> Sync POST to CommandServive is OK!");
        else
            Console.WriteLine("--> Sync POST to CommandServive is NOT OK!");
    }
}
