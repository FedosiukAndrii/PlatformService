using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService(IPlatformRepository repository, IMapper mapper) : GrpcPlatform.GrpcPlatformBase
{
    public async override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();

        var platforms = await repository.GetAllPlatformsAsync();

        response.Platform.AddRange(mapper.Map<IEnumerable<GrpcPlatformModel>>(platforms));

        return response;
    }
}