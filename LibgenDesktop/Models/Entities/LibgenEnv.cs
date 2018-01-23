using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LibgenDesktop.Models.Entities
{
    // to access this class as enviroment string
    internal abstract partial class LibgenObject
    {
        private Dictionary<string, object> env;
        private static Dictionary<string, string> env_map = new Dictionary<string, string>() {
                    // Mapping of good known property to enviroment name
                    { "Md5Hash", "md5"},
                    { "CoverUrl", "cover-url"},
                    { "Format", "ext" },
                    { "Doi", "doi"}, // to protect existing config
                };

        private MatchEvaluator env_me;
        private static Regex env_rx = new Regex(@"{([^{}]+)}");
        private string envRep(Match match)
        {
            string name = match.Groups[1].Value;
            object res;

            if (env == null)
            {
                env = new Dictionary<string, object>(8) {
                    { "id", LibgenId },
                    { "thousand-bucket", LibgenId / 1000 * 1000},
                    { "zound",  (LibgenId / 1000).ToString("D4")},
                };

                foreach (var prop in env_map)
                {
                    var var = GetType().GetProperty(prop.Key);
                    if (var != null)
                        env[prop.Value] = var.GetValue(this);
                }

                // extra lowercase alt-env
                if (env.TryGetValue("md5",out res)) env["md5-lc"] = res.ToString().ToLower();
                if (env.TryGetValue("ext",out res)) env["ext-lc"] = res.ToString().ToLower();
            }

            res = env.TryGetValue(name, out res) ? res :
                GetType().GetProperty(name)?.GetValue(this) ??
                match;

            return res.ToString();
        }
        public string Env(string template)
        {
            if (String.IsNullOrWhiteSpace(template))
                return null;

            if(env_me == null)
                env_me = new MatchEvaluator(envRep);
            return env_rx.Replace(template, env_me);
        }
    }
}
