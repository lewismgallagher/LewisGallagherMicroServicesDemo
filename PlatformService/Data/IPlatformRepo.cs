using PLatformService.Models;

namespace PLatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        Platform getPlatformById(int id);
        void CreatePlatform(Platform plat);
        
    }
}