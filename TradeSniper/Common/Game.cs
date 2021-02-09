using System.Diagnostics;
using System.Linq;

namespace TradeSniper.Common
{
    public static class Game
    {
        public static Process Proc
        {
            get
            {
                if (_process == null)
                {
                    var process = Process.GetProcessesByName("PathOfExile_x64").FirstOrDefault();
                    if (process == null)
                        return null;

                    _process = process;
                }

                return _process;
            }
        }

        public static void SendChat(string text)
        {
            Interop.SetForegroundWindow(Proc.MainWindowHandle);
            Interop.SetActiveWindow(Proc.MainWindowHandle);

            Input.InjectKey(Keys.Enter);
            Input.SimulateTextEntry(text);

            Thread.Sleep(45);
            Input.InjectKey(Keys.Enter);
        }

        public static string FmtGameMessage(Item item, Listing listing)
        {
            var sellerName = listing.Account.LastCharacterName;
            var itemName = $"{item.Name} {item.BaseType}".TrimStart();
            var itemStock = item.StackSize ?? 1;
            var league = item.League;
            var price = listing.Price;
            var priceAmount = price.Amount * itemStock;
            var currency = price.Currency;

            if (currency.Equals("chaos"))
                currency = "Chaos Orb";

            var itemFmt = $"{itemStock} {itemName} for my {priceAmount} {currency} in {league}.";
            return $"@{sellerName} Hi, I'd like to buy your {itemFmt}";
        }

        static Process _process;
    }
}