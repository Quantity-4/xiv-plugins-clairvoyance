using System;
using Clairvoyance.Libraries;
using Clairvoyance.UI;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using static Clairvoyance.Libraries.Helper;

namespace Clairvoyance
{
    public class Clairvoyance : IDalamudPlugin
    {
        public string Name => "Clairvoyance";
        public bool Disable;
        private bool refresh;

        public static Clairvoyance? Plugin { get; private set; }
        public static Configuration? Configuration { get; private set; }

        private readonly FrameworkHandler frameworkHandler;

        public Clairvoyance(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;

            Initialize(this, pluginInterface);

            Configuration = (Configuration)PluginInterface.GetPluginConfig()! ?? new();
            Configuration.Initialize();

            // Framework handler
            this.frameworkHandler = new FrameworkHandler();

            // User Interface
            PluginInterface.UiBuilder.Draw += ConfigUI.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += ConfigUI.ToggleVisibility;


            Framework.Update += this.FrameworkOnOnUpdateEvent;
        }

        void FrameworkOnOnUpdateEvent(IFramework framework)
        {
            if (this.Disable)
            {
                this.Disable = false;
                this.refresh = false;
            }
            else if (this.refresh)
            {
                this.Disable = true;
            }
            else
            {
                // TODO: ADD HERE/UNCOMMENT TO UPDATE ON EVERY FRAME
                // frameworkHandler.Update();
            }
        }

        // --- Commands ---
        [Command("/playerlist")]
        [Aliases("/pl")]
        [HelpMessage("Lists all the players in the area, and their coordinates")]
        private void OnPlayerList(string command, string args)
        {
            frameworkHandler.Update();
        }

        // --- Dispose ---

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            //Config.Save();
            PluginInterface.UiBuilder.Draw -= ConfigUI.Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= ConfigUI.ToggleVisibility;

            // Static
            Helper.Dispose();

            // Dynamic
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
