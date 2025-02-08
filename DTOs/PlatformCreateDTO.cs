using System.ComponentModel.DataAnnotations;

namespace PlatformService.DTOs;

public record PlatformCreateDTO(
    [Required] string Name,
    [Required] string Publisher,
    [Required] string Cost
);
