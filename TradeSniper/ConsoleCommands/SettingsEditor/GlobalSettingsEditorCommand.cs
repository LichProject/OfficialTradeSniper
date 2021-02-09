using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public class GlobalSettingsEditor : SettingsEditor
    {
        public GlobalSettingsEditor(ConsoleCommandConfiguration config)
            : base(config, config.GlobalSettings)
        {
        }

        #region Overrides of BaseConsoleCommand

        public override string Description => "Изменить глобальные настройки";
        public override string Alias => "editglobal";

        #endregion
    }
}