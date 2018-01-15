using System;
using System.IO;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Settings
{
    internal static class MirrorStorage
    {
        public static Mirrors LoadMirrors(string mirrorsFilePath)
        {
            Mirrors result;
            if (File.Exists(mirrorsFilePath))
            {
                result = DeserializeFile(mirrorsFilePath);
            }
            else
            {
                throw new Exception($"Cannot find {mirrorsFilePath}");
            }
            return result;
        }

        private static Mirrors DeserializeFile(string mirrorsFilePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader streamReader = new StreamReader(mirrorsFilePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                return jsonSerializer.Deserialize<Mirrors>(jsonTextReader);
            }
        }
    }
}
