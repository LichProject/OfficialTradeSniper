using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveSearchEngine.Interfaces;
using LiveSearchEngine.Models;
using TradeSniper.Interfaces;
using TradeSniper.Models;

namespace TradeSniper
{
    public class ConsoleMenu
    {
        public ConsoleMenu(ILogger logger)
        {
            Logger = logger;
            Console.OutputEncoding = Encoding.UTF8;
        }

        public ILogger Logger { get; }

        public IReadOnlyList<MenuAction> Menu => _menuActions.FindAll(x => x.CanExecute).AsReadOnly();

        public void AddParagraph(string description, string alias, Action action, Func<bool> condition)
        {
            _menuActions.Add(new MenuAction(description, alias, action, condition));
        }

        public void AddCommand(IConsoleCommand command)
        {
            AddParagraph(command.Description, command.Alias, command.Execute, command.ExecuteCondition);
        }

        public void AddCommand<T>() where T : IConsoleCommand
        {
            var command = (IConsoleCommand) Activator.CreateInstance(typeof(T), _config ?? new CommandConfiguration());
            AddCommand(command);
        }

        public void UseCommandConfiguration(CommandConfiguration config)
        {
            _config = config;
        }

        public void Run()
        {
            while (true)
            {
                ShowMenu();

                Input:
                var input = Console.ReadLine();
                var targetMenu = _menuActions.FirstOrDefault(x => x.IsTargetCommand(input));
                if (targetMenu == null)
                {
                    Logger.Error("Не удалось найти пункт меню с этим кодом.");
                    goto Input;
                }

                targetMenu.Execute();
                goto Input;
            }
        }

        public void ShowMenu()
        {
            Logger.Info("////////////////////////////////");

            foreach (var menu in Menu)
            {
                Logger.Info("[{0}] {1}", menu.CommandAlias, menu.Description);
            }

            Logger.Info("////////////////////////////////");
        }

        CommandConfiguration _config;
        readonly List<MenuAction> _menuActions = new List<MenuAction>();
    }
}