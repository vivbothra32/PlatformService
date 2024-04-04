using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        Task<HttpResponseMessage> SendDatatoCommandService(PlatformReadDto platformReadDto);
    }
}