using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Controllers;

[ApiController, Route("api/[controller]")]
public class PlatformsController(IPlatformRepository platformRepository, ICommandDataClient commandDataClient, IMapper mapper, IOutboxRepository outboxRepository) : ControllerBase
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
    public async Task<IActionResult> CreatePlatform([FromBody, Required] PlatformCreateDTO platformCreateDTO)
    {
        var platformModel = mapper.Map<Platform>(platformCreateDTO);

        // Start a transaction
        await using var transaction = await platformRepository.BeginTransactionAsync();

        try
        {
            // Save the platform
            await platformRepository.CreatePlatfromAsync(platformModel);
            await platformRepository.SaveChangesAsync();

            var platformReadDTO = mapper.Map<PlatformReadDTO>(platformModel);
            var platformPublishedDTO = mapper.Map<PlatformPublishedDTO>(platformReadDTO);

            // Create and save the outbox message
            var outboxMessage = OutboxMessage.Create("PlatformPublished", platformPublishedDTO);
            await outboxRepository.AddOutboxMessageAsync(outboxMessage);
            await outboxRepository.SaveChangesAsync();

            // Commit the transaction
            await transaction.CommitAsync();

            // Send synchronous HTTP notification outside the transaction
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
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Transaction failed: {ex.Message}");
            throw;
        }
    }
}