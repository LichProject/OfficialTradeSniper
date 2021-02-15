using LiveSearchEngine.Models;
using TradeSniper.Models;
using TradeSniper.Settings;

namespace TradeSniper.ConsoleCommands
{
    public class SniperSettingsEditorCommand : SettingsEditor<SniperSettings>
    {
        public SniperSettingsEditorCommand(CommandConfiguration config)
            : base(config, config.SniperSettings)
        {
        }

        #region Overrides of BaseConsoleCommand

        public override string Description => "Изменить настройки снайпера.";
        public override string Alias => "editsniper";

        #endregion
    }
}