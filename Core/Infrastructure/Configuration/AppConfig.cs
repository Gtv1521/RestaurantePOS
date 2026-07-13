using Microsoft.Extensions.Configuration;
using System.IO;
using MiComanderaApp.Models;

namespace MiComanderaApp.Services
{
    public static class AppConfig
    {
        public static ApiSettings ApiSettings { get; }

        static AppConfig()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ApiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
        }
    }
}