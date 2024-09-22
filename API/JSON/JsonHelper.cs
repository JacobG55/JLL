using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JLL.API.JSON
{
    public class JsonHelper
    {
        public static string AsJson<T>(T type, bool formatted = true)
        {
            return JsonConvert.SerializeObject(type, formatted ? Formatting.Indented : Formatting.None);
        }

        public static bool ConvertJson<T>(string json, out T output)
        {
            output = JsonConvert.DeserializeObject<T>(json);
            if (output != null)
            {
                return true;
            }
            return false;
        }

        public static string[] GetJsonFilesInDirectory(string directory, bool searchSubs = true)
        {
            return GetFilesInDirectory(directory, new List<string> { ".json" }, searchSubs);
        }

        public static string[] GetFilesInDirectory(string directory, List<string> extensions, bool searchSubs = true)
        {
            return Directory.GetFiles(directory, "*.*", searchSubs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Where(file => extensions.Any(extension => file.EndsWith(extension))).ToArray();
        }

        public static List<string> GetTextFromFiles(string[] files)
        {
            List<string> contents = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    contents.Add(File.ReadAllText(files[i]));
                }
                catch
                {
                    JLogHelper.LogWarning($"Something went wrong reading: {files[i]}");
                }
            }
            return contents;
        }

        public static List<T> ParseJsonFiles<T>(string[] files)
        {
            List<T> contents = new List<T>();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    if (ConvertJson(File.ReadAllText(files[i]), out T obj))
                    {
                        contents.Add(obj);
                    }
                }
                catch
                {
                    JLogHelper.LogWarning($"Something went wrong reading: {files[i]}");
                }
            }
            return contents;
        }

        public static List<T> ParseJsonFiles<T>(string directory)
        {
            return ParseJsonFiles<T>(GetJsonFilesInDirectory(directory));
        }
    }
}
