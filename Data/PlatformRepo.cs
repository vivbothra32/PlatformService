using PlatformService.Models;
using System.Linq;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePlatform(Platform platform)
        {
            _context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms;
        }

        public Platform GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(c => c.Id == id);
        }

        public bool SaveChanges()
        {
            if(_context.SaveChanges() > 0){
                return true;
            }
            return false;
        }
    }
}