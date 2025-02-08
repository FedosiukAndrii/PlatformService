using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[ApiController, Route("api/[controller]")]
public class PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlatforms()
    {
        Console.WriteLine("Getting platforms");

        var platforms = await platformRepository.GetAllPlatformsAsync();

        return Ok(mapper.Map<IEnumerable<PlatformReadDTO>>(platforms));
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public async Task<IActionResult> GetPlatformById(int id)
    {
        Console.WriteLine("Getting platform by id");

        var platform = await platformRepository.GetPlatformByIdAsync(id);

        if (platform is not null)
            return Ok(mapper.Map<PlatformReadDTO>(platform));
        else
            return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform(PlatformCreateDTO platformCreateDTO)
    {
        if (platformCreateDTO is not null)
        {
            var platformModel = mapper.Map<Platform>(platformCreateDTO);

            await platformRepository.CreatePlatfromAsync(platformModel);
            await platformRepository.SaveChangesAsync();

            var platformReadDTO = mapper.Map<PlatformReadDTO>(platformModel);

            try
            {
                await commandDataClient.SendPlatformToCommand(platformReadDTO);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDTO.Id }, platformReadDTO);
        }
        else
            return BadRequest();
    }
}
