using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using LiveSearchEngine.Models;
using Newtonsoft.Json;
using RestSharp.Extensions;
using TradeSniper.Common;
using TradeSniper.Common.Utils;
using TradeSniper.Models;

namespace TradeSniper.ConsoleCommands
{
    public abstract class SettingsEditor<T> : BaseConsoleCommand where T : JsonSettings
    {
        protected SettingsEditor(CommandConfiguration config, T settings)
            : base(config)
        {
            Settings = settings;
        }

        protected T Settings { get; }

        #region Overrides of BaseConsoleCommand

        public override void Execute()
        {
            Logger.Info("Изменение настроек для {0}", Settings.GetType().Name);
            Logger.Info("Выбери пункт который нужно изменить: ");

            var properties = Settings.GetType().GetProperties();

            var i = 0;
            foreach (var p in properties)
            {
                if (p.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null)
                    continue;
                
                var desc = p.GetAttribute<DescriptionAttribute>()?.Description ?? p.Name;
                Logger.Info("\t\t[{0}] <{1}>: {2}", i++, desc, p.GetValue(Settings) ?? "null");
            }

            Retry:
            Logger.Info("Введи индекс нужной настройки (указаны выше):");
            Logger.Info("Ввод любого текста приведет к выходу из этого пункта меню.");
            if (!ConsoleUtils.GetIntegerFromLine(out int index))
            {
                Logger.Info("Вышли в меню");
                return;
            }

            var property = properties.ElementAtOrDefault(index);
            if (property == null)
            {
                Logger.Error("Не удалось найти поле с этим индексом ({0}).", index);
                goto Retry;
            }

            Logger.Info("1) Изменить значение.");
            Logger.Info("2) Выбрать другое.");

            if (!ConsoleUtils.GetIntegerFromLine(out int key))
                goto Retry;

            switch (key)
            {
                case 1:
                    Logger.Info("Введи новое значение:");
                    var input = Console.ReadLine();
                    try
                    {
                        var value = Convert.ChangeType(input, property.PropertyType);
                        property.SetValue(Settings, value);

                        Logger.Info("\t\tНовое значение для {0} : {1}", property.Name, value);
                        Settings.Save();
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Не удалось изменить значение (настройки не были сохранены), причина:");
                        Logger.Error(e.ToString());
                    }

                    goto Retry;

                case 2: goto Retry;
            }
        }

        #endregion
    }
}