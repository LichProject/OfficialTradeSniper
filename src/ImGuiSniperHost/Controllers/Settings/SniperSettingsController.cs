using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using ImGuiSniperHost.Common;
using ImGuiSniperHost.Models;
using ImGuiSniperHost.Settings;
using LiveSearchEngine.Models;
using LiveSearchEngine.Models.Default;

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
                if (!CreationModels.TryGetValue(item, out var model))
                    CreationModels[item] = model = new SniperItemCreationModel(item);

                if (ImGui.CollapsingHeader(item.Description))
                {
                    ImGui.Text(item.SearchUrlWrapper.SearchUrl);

                    ShowInputForModel(model, true);

                    DrawSaveButton(
                        Settings,
                        item.Description,
                        () =>
                        {
                            item.SearchHash = model.Hash;
                            item.League = model.League;
                        });

                    ImGui.SameLine();

                    if (ImGui.Button("Delete##" + item.Description))
                        RemoveValidateSniperItem(item);
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

            try
            {
                ShowInputForModel(CreationModel);

                if (ImGui.Button("Add"))
                {
                    AddValidateSniperItem();
                    ImGui.CloseCurrentPopup();
                }
            }
            finally
            {
                ImGui.EndPopup();
            }
        }

        void AddValidateSniperItem()
        {
            if (!ValidateSniperItem(CreationModel))
                return;

            var model = CreationModel.ToSniperItemObject();

            Settings.AddSniper(model);
            Settings.Save();

            CreationModel.Reset();

            Logger.Info("Created new sniper item ({0}: {1})", model.Description, model.SearchUrlWrapper.SearchUrl);
        }

        bool ValidateSniperItem(SniperItemCreationModel model)
        {
            if (new[] {model.Description, model.Hash, model.League}.Any(string.IsNullOrEmpty))
                return false;

            if (Settings.SniperItems.Any(x => x.SearchHash == model.Hash || x.Description == model.Description))
                return false;

            return true;
        }

        void RemoveValidateSniperItem(SniperItem item)
        {
            Settings.RemoveSniper(item);
            Settings.Save();
            
            Logger.Info("Deleted sniper item with hash: {0}", item.SearchHash);
        }

        void ShowInputForModel(SniperItemCreationModel model, bool edit = false)
        {
            if (!edit)
                ImGui.InputText($"{nameof(SniperItem.Description)}##add", ref model.Description, 256);

            ImGui.InputText($"{nameof(SniperItem.SearchHash)}##{model.Description}{edit}", ref model.Hash, 128);
            ImGui.InputText($"{nameof(SniperItem.League)}##{model.Description}{edit}", ref model.League, 128);
        }

        static ImGuiLogger Logger => ImGuiLogger.Instance;
        
        static readonly Dictionary<SniperItem, SniperItemCreationModel> CreationModels =
            new Dictionary<SniperItem, SniperItemCreationModel>();

        static readonly SniperItemCreationModel CreationModel = new SniperItemCreationModel();
    }
}