using Clairvoyance.lib;
using Dalamud.Configuration;

namespace Clairvoyance
{
    public partial class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }

        public void Initialize() { }

        public void Save()
        {
            Helper.PluginInterface.SavePluginConfig(this);
        }
    }
}
