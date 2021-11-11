using System.Threading.Tasks;
using PlatformService.Dtos;

namespace PlatformService.SynchDataServices.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto platformReadDto);
    }
}