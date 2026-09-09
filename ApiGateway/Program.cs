using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                       .ConfigureAppConfiguration((hostingContext, config) =>
                       {
                           //Added on 31_Mar_25
                           var env = hostingContext.HostingEnvironment.EnvironmentName;

                           JObject globalConfig;

                           string[] routeFiles;



                           globalConfig = JObject.Parse(File.ReadAllText("ocelot.global.json"));
                           routeFiles = Directory.GetFiles("ocelot-config", "*.json");


                           var allReRoutes = new JArray();
                           foreach (var file in routeFiles)
                           {
                               var json = JObject.Parse(File.ReadAllText(file));
                               var reRoutes = (JArray)json["ReRoutes"];
                               if (reRoutes != null)
                               {
                                   allReRoutes.Merge(reRoutes);
                               }
                           }

                           // Combine GlobalConfiguration and ReRoutes
                           globalConfig["ReRoutes"] = allReRoutes;

                           // Write the combined configuration to a temporary file
                           var mergedConfigPath = Path.Combine(Path.GetTempPath(), "ocelot.merged.json");
                           File.WriteAllText(mergedConfigPath, globalConfig.ToString());

                           // Load the merged configuration into Ocelot
                           config.AddJsonFile(mergedConfigPath, optional: false, reloadOnChange: true);
                       })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
