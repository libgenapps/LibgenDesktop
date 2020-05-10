using System;
using System.Collections.Generic;
using System.Globalization;

namespace LibgenDesktop.Models.Entities
{
    internal class DatabaseMetadata
    {
        internal class FieldDefinition
        {
            public FieldDefinition(string fieldName, Func<DatabaseMetadata, string> getter, Action<DatabaseMetadata, string> setter)
            {
                FieldName = fieldName;
                Getter = getter;
                Setter = setter;
            }

            public string FieldName { get; }
            public Func<DatabaseMetadata, string> Getter { get; }
            public Action<DatabaseMetadata, string> Setter { get; }
        }

        static DatabaseMetadata()
        {
            FieldDefinitions = new Dictionary<string, FieldDefinition>();
            AddField("AppName", metadata => metadata.AppName, (metadata, value) => metadata.AppName = value);
            AddField("Version", metadata => metadata.Version, (metadata, value) => metadata.Version = value);
            AddField("NonFictionFirstImportComplete", metadata => metadata.NonFictionFirstImportComplete.ToString(),
                (metadata, value) => metadata.NonFictionFirstImportComplete = value == Boolean.TrueString);
            AddField("FictionFirstImportComplete", metadata => metadata.FictionFirstImportComplete.ToString(),
                (metadata, value) => metadata.FictionFirstImportComplete = value == Boolean.TrueString);
            AddField("SciMagFirstImportComplete", metadata => metadata.SciMagFirstImportComplete.ToString(),
                (metadata, value) => metadata.SciMagFirstImportComplete = value == Boolean.TrueString);
        }

        public DatabaseMetadata()
        {
            AppName = null;
            Version = null;
            NonFictionFirstImportComplete = null;
            FictionFirstImportComplete = null;
            SciMagFirstImportComplete = null;
        }

        public static Dictionary<string, FieldDefinition> FieldDefinitions { get; }

        public string AppName { get; set; }
        public string Version { get; set; }
        public bool? NonFictionFirstImportComplete { get; set; }
        public bool? FictionFirstImportComplete { get; set; }
        public bool? SciMagFirstImportComplete { get; set; }

        public Version GetParsedVersion()
        {
            if (!System.Version.TryParse(Version, out Version result))
            {
                return null;
            }
            return result;
        }

        private static void AddField(string fieldName, Func<DatabaseMetadata, string> getter, Action<DatabaseMetadata, string> setter)
        {
            FieldDefinitions.Add(fieldName.ToLower(), new FieldDefinition(fieldName, getter, setter));
        }
    }
}
