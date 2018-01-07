using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace LibgenDesktop.Models.Settings
{
    internal static class MirrorStorage
    {
        public static Mirrors LoadMirrors(string mirrorsFilePath)
        {
            Mirrors result;
            try
            {
                if (File.Exists(mirrorsFilePath))
                {
                    result = DeserializeFile(mirrorsFilePath);
                }
                else
                {
                    result = SaveDefaultMirrors(mirrorsFilePath);
                }
            }
            catch
            {
                result = SaveDefaultMirrors(mirrorsFilePath);
            }
            if (!result.Any())
            {
                result = SaveDefaultMirrors(mirrorsFilePath);
            }
            return result;
        }

        private static Mirrors SaveDefaultMirrors(string mirrorsFilePath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream defaultMirrorsResourceStream = executingAssembly.GetManifestResourceStream(@"LibgenDesktop.Resources.default_mirrors.config"))
            using (FileStream outputFileStream = new FileStream(mirrorsFilePath, FileMode.Create))
            {
                defaultMirrorsResourceStream.CopyTo(outputFileStream);
            }
            return DeserializeFile(mirrorsFilePath);
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
