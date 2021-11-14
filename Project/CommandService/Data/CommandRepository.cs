using System;
using System.Collections.Generic;
using System.Linq;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _appDbContext;

        public CommandRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.PlatformId = platformId;
            _appDbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            _appDbContext.Add(platform);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return _appDbContext.Platforms.Any(x => x.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _appDbContext.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _appDbContext.Commands.FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
        }

        public IEnumerable<Command> GetCommandsByPlatformId(int platformId)
        {
            return _appDbContext.Commands.Where(x => x.PlatformId == platformId).OrderBy(x => x.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _appDbContext.Platforms.Any(x => x.Id == platformId);
        }

        public bool SaveChanges()
        {
            return _appDbContext.SaveChanges() >= 0;
        }
    }
}