using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project2
{//History save and load
    class HistorySandL
    {
        private static string GetFilePath(string userName)
        {

            var safeName = userName.Trim().ToLower().Replace(" ", "_");
            return Path.Combine(FileSystem.AppDataDirectory, "$movie_history_{safeName} .json");
        }
        public static List<MovieHistory> Load(string userName)
        {
            var path = GetFilePath(userName);
            if (!File.Exists(path))
                return new List<MovieHistory>();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<MovieHistory>>(json)
                ?? new List<MovieHistory>();
        }

        public static void Save(string userName, List<MovieHistory> history)
        {
            var path = GetFilePath(userName);
            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            File.WriteAllText(path, json);
        }
    }
}
