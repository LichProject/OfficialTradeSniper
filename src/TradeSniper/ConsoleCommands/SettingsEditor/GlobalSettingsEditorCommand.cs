using LiveSearchEngine.Models;
using TradeSniper.Models;
using TradeSniper.Settings;

namespace TradeSniper.ConsoleCommands
{
    public class GlobalSettingsEditorCommand : SettingsEditor<GlobalSettings>
    {
        public GlobalSettingsEditorCommand(CommandConfiguration config)
            : base(config, config.GlobalSettings)
        {
        }

        #region Overrides of BaseConsoleCommand

        public override string Description => "Изменить глобальные настройки";
        public override string Alias => "editglobal";

        #endregion
    }
}