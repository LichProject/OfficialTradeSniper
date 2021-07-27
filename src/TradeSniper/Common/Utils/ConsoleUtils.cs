using System;

namespace TradeSniper.Common.Utils
{
    public static class ConsoleUtils
    {
        public static bool GetIntegerFromLine(out int index)
        {
            var input = Console.ReadLine();
            return int.TryParse(input, out index);
        }
    }
}