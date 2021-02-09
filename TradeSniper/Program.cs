using System;
using TradeSniper.Common;

namespace TradeSniper
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            var procId = game.LookupProcessId();
            
            // var gameId = Process
            // var officialSniper = new OfficialTradeSniper();
      TradeSniperew LiveSearch(officialSniper);
            // ls.OnItemFound += OnItemFound_Callback;
            // ls.Run();
            
            
            
            Console.WriteLine("Hello World!");
        }
        
        // OnItemFound_Callback(Item item, Price price)
        // {
        // var sellerName = item.AccountName;
        // var itemName = $"{item.Name} {item.BaseType}".TrimStart();
        // var stock = item.StackSize;
        // var league = item.League;
        //
        // Game.SetWindowForeground();
        // Game.SendChatFmt("@{0} Hi, I'd like to buy your {1} {2} for my {3} {4} in {5}.",
        // sellerName, stock, itemName, price.Amount, price.OrbName, league)
        // }
    }
}