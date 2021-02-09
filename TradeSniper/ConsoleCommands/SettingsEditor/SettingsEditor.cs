using System;
using System.Collections.Generic;
using TradeSniper.Common;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public abstract class SettingsEditor : BaseConsoleCommand
    {
        protected SettingsEditor(ConsoleCommandConfiguration config)
            : base(config)
        {
        }

        public List<JsonSettings> Settings { get; } = new List<JsonSettings>();

        #region Overrides of BaseConsoleCommand

        public override void Execute()
        {
            Logger.Info("Какие настройки изменить?");
        }

        #endregion

        public abstract void
    }
}