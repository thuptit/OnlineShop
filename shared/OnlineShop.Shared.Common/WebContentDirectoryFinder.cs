using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Shared.Common
{
    public static class WebContentDirectoryFinder
    {
        public static string CalculateContentRootFolder<TModule>(string serviceName) where TModule : class
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(TModule).GetAssembly().Location);
            if (coreAssemblyDirectoryPath == null)
            {
                throw new Exception("Could not find location assembly!");
            }

            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!DirectoryContains(directoryInfo.FullName, "OnlineShop.sln"))
            {
                if (directoryInfo.Parent == null)
                {
                    throw new Exception("Could not find content root folder!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            var webApiFolder = Path.Combine(directoryInfo.FullName, serviceName, $"OnlineShop.{serviceName}.Api");
            if (Directory.Exists(webApiFolder))
            {
                return webApiFolder;
            }

            //var webHostFolder = Path.Combine(directoryInfo.FullName, serviceName, $"OnlineShop.{serviceName}.EntityFrameworkCore");
            //if (Directory.Exists(webHostFolder))
            //{
            //    return webHostFolder;
            //}

            throw new Exception("Could not find root folder of the web project!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
