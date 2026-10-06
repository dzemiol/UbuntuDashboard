using UbuntuDashboard.Models;

namespace UbuntuDashboard.Services
{
    public class ServicesService
    {
        private readonly List<ServiceModel> _services = new()
        {
            new ServiceModel
            {
                Name = "Minecraft Server",
                Description = "Serwer gry Survival",
                Icon = "🎮",
                IsActive = false
            },
            new ServiceModel
            {
                Name = "FinanceApp",
                Description = "Aplikacja Finansowa",
                Icon = "💰",
                IsActive = false,
                Port = 5005
            }
        };

        public List<ServiceModel> GetServices()
        {
            return _services;
        }

        public void StartService(ServiceModel service)
        {
            service.IsActive = true;
        }

        public void StopService(ServiceModel service)
        {
            service.IsActive = false;
        }
    }
}
