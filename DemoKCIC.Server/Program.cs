using Microsoft.Extensions.Configuration;

namespace DemoKCIC.Server
{
    public class ConfigurationService
    {
        public IConfiguration Configuration { get; }

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
    }
}
