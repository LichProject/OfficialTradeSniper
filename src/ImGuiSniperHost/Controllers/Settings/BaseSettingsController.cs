using System;
using System.Collections.Generic;
using System.Drawing;
using ImGuiNET;
using ImGuiSniperHost.Common;

namespace ImGuiSniperHost.Controllers.Settings
{
    public abstract class BaseSettingsController : IDrawController
    {
        public TimeSpan SaveTextFadeTimeout { get; protected set; } = TimeSpan.FromSeconds(10);

        #region Implementation of IDrawController

        public abstract void Draw();

        #endregion

        protected void DrawSaveButton(JsonSettings settings, string id, Action preSaveCallback = null)
        {
            var td = DateTime.Now.TimeOfDay;
            
            if (ImGui.Button("Save##" + id))
            {
                _savedLabels[id] = td;
                preSaveCallback?.Invoke();
                settings.Save();
            }
            
            if (!_savedLabels.TryGetValue(id, out var savedTd))
                return;
            
            if (savedTd > td.Subtract(SaveTextFadeTimeout))
            {
                ImGui.SameLine();
                ImGui.TextColored(Color.Green.ToImGuiVec4(), "Saved!");
            }
        }

        readonly Dictionary<string, TimeSpan> _savedLabels = new Dictionary<string, TimeSpan>();
    }
}