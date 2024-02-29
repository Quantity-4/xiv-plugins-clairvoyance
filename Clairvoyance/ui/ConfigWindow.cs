using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;

using ImGuiNET;


namespace Clairvoyance.UI
{
    public static class ConfigUI
    {
        public static bool isVisible = false;

        public static void ToggleVisibility()
        {
            isVisible = !isVisible;
        }

        public static void Draw()
        {
            if (!isVisible) return;

            ImGui.SetNextWindowSize(new Vector2(400, 0) * ImGuiHelpers.GlobalScale);
            ImGui.Begin("Clairvoyance", ref isVisible, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Clairvoyance - Main");

            // --- Tool config options ---

            ImGui.End();
        }
    }
}
