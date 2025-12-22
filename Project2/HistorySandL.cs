using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project2
{//History save and load
    class HistorySandL
    {
        //creates the file path for history
        private static string GetFilePath(string userName)
        {
            //mesnt to make the username safe so no spaces and not capital letters
            var safeName = userName.Trim().ToLower().Replace(" ", "_");
            //
            return Path.Combine(FileSystem.AppDataDirectory, "$movie_history_{safeName}.json");
        }
        //Load history
        public static List<MovieHistory> Load(string userName)
        {
            var path = GetFilePath(userName);


            if (!File.Exists(path))
                return new List<MovieHistory>();

            var json = File.ReadAllText(path);

         //deserialize JSON to MovieHistory
            return JsonSerializer.Deserialize<List<MovieHistory>>(json)
                ?? new List<MovieHistory>();
        }

        //save history
        public static void Save(string userName, List<MovieHistory> history)
        {

            var path = GetFilePath(userName);
            //convert history to JSON
            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            //Write JSON to file
            File.WriteAllText(path, json);
        }
    }
}
