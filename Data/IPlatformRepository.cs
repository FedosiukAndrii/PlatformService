﻿using Microsoft.EntityFrameworkCore.Storage;
using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepository
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    Task<bool> SaveChangesAsync();
        
    Task<IEnumerable<Platform>> GetAllPlatformsAsync();

    Task<Platform> GetPlatformByIdAsync(int id);

    Task CreatePlatfromAsync(Platform platform);
}
