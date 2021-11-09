using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _appDbContext;
        public PlatformRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Create(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            _appDbContext.Add(platform);
        }

        public IEnumerable<Platform> GetAll()
        {
            return _appDbContext.Platforms.ToList();
        }

        public Platform GetById(int id)
        {
            return _appDbContext.Platforms.FirstOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return _appDbContext.SaveChanges() >= 0;
        }
    }
}