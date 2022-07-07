using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Helper
{
    public static class CsgoServerHelper
    {
        /// <summary>
        /// Takes a filePath from Resources/Raw and converts any .cfg file into a single console command.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Return console command for cs:go servers</returns>
        public static string GetCfg(string filePath)
        {
            string[] newlines = { "\n", "\r" };
            List<string> commands = Task.Run(() => LoadMauiAsset(filePath)).Result.Split(newlines, StringSplitOptions.RemoveEmptyEntries).ToList();

            for(int i = 0; i < commands.Count; i++)
            {
                string cmd = commands[i];

                commands[i] = cmd.Split("//")[0].Trim();
            }

            commands.RemoveAll((s) => string.IsNullOrEmpty(s));

            string result = string.Join("; ", commands);
            return result += ";";
        }

        /// <summary>
        /// Loads a file from Resources/Raw using MauiAssets.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string</returns>
        public static async Task<string> LoadMauiAsset(string filePath)
        {
            using Stream fileStream = FileSystem.OpenAppPackageFileAsync(filePath).Result;
            using StreamReader reader = new StreamReader(fileStream);

            return await reader.ReadToEndAsync();
        }
    }
}
