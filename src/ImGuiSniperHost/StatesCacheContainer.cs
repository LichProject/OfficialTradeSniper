using System.Collections.Generic;

namespace ImGuiSniperHost
{
    public class StatesCacheContainer
    {
        public bool this[string key]
        {
            get
            {
                _vars.TryGetValue(key, out bool value);
                return value;
            }
            set => _vars[key] = value;
        }

        readonly Dictionary<string, bool> _vars = new Dictionary<string, bool>();
    }
}