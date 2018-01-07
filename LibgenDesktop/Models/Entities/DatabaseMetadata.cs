using System;
using System.Collections.Generic;

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
            AddField("Version", metadata => metadata.Version, (metadata, value) => metadata.Version = value);
        }

        public static Dictionary<string, FieldDefinition> FieldDefinitions { get; }

        public string Version { get; set; }

        private static void AddField(string fieldName, Func<DatabaseMetadata, string> getter, Action<DatabaseMetadata, string> setter)
        {
            FieldDefinitions.Add(fieldName.ToLower(), new FieldDefinition(fieldName, getter, setter));
        }
    }
}
