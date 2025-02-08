namespace PlatformService.DTOs;

public record PlatformReadDTO(
    int Id,
    string Name,
    string Publisher,
    string Cost
);
