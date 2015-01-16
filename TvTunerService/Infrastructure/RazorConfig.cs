using System.Collections.Generic;
using Nancy.ViewEngines.Razor;

namespace TvTunerService {
    
    public class RazorConfig : IRazorConfiguration {
        public IEnumerable<string> GetAssemblyNames() {
            yield return "EZTV";
        }

        public IEnumerable<string> GetDefaultNamespaces() {
            yield return "EZTV";
        }

        public bool AutoIncludeModelNamespace { get { return true; } }
    }
     
}