using System;

namespace TradeSniper.Interfaces
{
    public interface IConsoleCommand
    {
        string Description { get; }
        string Alias { get; }
        
        Func<bool> ExecuteCondition { get; }

        void Execute();
    }
}