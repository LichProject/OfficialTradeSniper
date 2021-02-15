using System;
using System.Linq;
using ImGuiNET;
using ImGuiSniperHost.Common;
using ImGuiSniperHost.Settings;
using LiveSearchEngine.Models;

namespace ImGuiSniperHost.Controllers.Settings
{
    public class SniperSettingsController : BaseSettingsController
    {
        SniperSettings Settings => SniperSettings.Instance;

        #region Overrides of BaseSettingsController

        const string CreationModalWindowTitle = "Add new item";

        public override void Draw()
        {
            foreach (var item in Settings.SniperItems.ToList())
            {
                if (ImGui.TreeNodeEx(item.Description + "##" + item.SearchHash))
                {
                    ImGui.LabelText(nameof(item.LiveUrlWrapper), item.LiveUrlWrapper.SearchUrl);
                    ImGui.LabelText(nameof(item.League), item.League);

                    if (ImGui.Button("Delete##" + item.SearchHash))
                    {
                        Settings.RemoveSniper(item);
                        Settings.Save();
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.NewLine();
            
            if (ImGui.Button("Create new item"))
                ImGui.OpenPopup(CreationModalWindowTitle);

            DrawCreationModal();
        }

        #endregion

        void DrawCreationModal()
        {
            var opened = true;

            if (!ImGui.BeginPopupModal(CreationModalWindowTitle, ref opened, GlobalStyle.DefaultWindowFlags))
                return;

            ImGui.InputText(nameof(SniperItem.Description), ref _modalDescription, 256);
            ImGui.InputText(nameof(SniperItem.SearchHash), ref _modalHash, 128);
            ImGui.InputText(nameof(SniperItem.League), ref _modalLeague, 128);

            if (ImGui.Button("Add"))
            {
                Settings.AddSniper(new SniperItem(_modalDescription, _modalHash, _modalLeague));
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        static string _modalDescription = string.Empty;
        static string _modalHash = string.Empty;
        static string _modalLeague = string.Empty;
    }
}