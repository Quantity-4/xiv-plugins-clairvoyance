using System;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Clairvoyance.lib;
using Clairvoyance.server;
using Clairvoyance.UI;


namespace Clairvoyance
{
    public class Clairvoyance : IDalamudPlugin
    {
        public string Name => "Clairvoyance";
        public bool Disable;
        private bool refresh;

        public static Clairvoyance? Plugin { get; private set; }
        public static Configuration? Configuration { get; private set; }

        private readonly FrameworkHandler _frameworkHandler;

        public Clairvoyance(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;

            Helper.Initialize(this, pluginInterface);

            Configuration = (Configuration)Helper.PluginInterface.GetPluginConfig()! ?? new();
            Configuration.Initialize();


            // Framework handler
            this._frameworkHandler = new FrameworkHandler();

            // User Interface
            Helper.PluginInterface.UiBuilder.Draw += ConfigUI.Draw;
            Helper.PluginInterface.UiBuilder.OpenConfigUi += ConfigUI.ToggleVisibility;

            Helper.Framework.Update += this.FrameworkOnOnUpdateEvent;
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
                _frameworkHandler.Update();
            }
        }

        // --- Commands ---
        [Command("/playerlist")]
        [Aliases("/pl")]
        [HelpMessage("Lists all the players in the area, and their coordinates")]
        private void OnPlayerList(string command, string args)
        {
            _frameworkHandler.Update();
        }

        [Command("/clearmapdata")]
        [Aliases("/clearmd")]
        [HelpMessage("Clears all the map data")]
        private void OnClearAllMapData(string command, string args)
        {
            _frameworkHandler.ClearAllMapData();
        }

        // --- Dispose ---

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            //Config.Save();
            Helper.PluginInterface.UiBuilder.Draw -= ConfigUI.Draw;
            Helper.PluginInterface.UiBuilder.OpenConfigUi -= ConfigUI.ToggleVisibility;

            // Static
            Helper.Dispose();

            // Dynamic
            _frameworkHandler.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}