using System;

namespace LiveSearchEngine.Models
{
    public class MenuAction
    {
        public MenuAction(string description, string commandAlias, Action action, Func<bool> condition)
        {
            Description = description;
            CommandAlias = commandAlias;
            Action = action;
            Condition = condition;
        }

        public string Description { get; }
        public string CommandAlias { get; }

        Action Action { get; }
        Func<bool> Condition { get; }

        public bool CanExecute => Condition == null || Condition();

        public void Execute()
        {
            if (!CanExecute || Action == null)
                return;

            Action();
        }

        public bool IsTargetCommand(string alias) => string.Equals(CommandAlias, alias, StringComparison.CurrentCultureIgnoreCase);
    }
}