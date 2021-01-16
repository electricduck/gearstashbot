using System;

namespace GearstashBot.Utilities
{
    public class GeneratorUtilities
    {
        private static Random random = new Random();

        public static string GenerateClockEmoji(DateTime date)
        {
            switch(date.Hour)
            {
                case 0:
                case 12:
                case 24:
                    return "🕛";
                case 1:
                case 13:
                    return "🕐";
                case 2:
                case 14:
                    return "🕑";
                case 3:
                case 15:
                    return "🕒";
                case 4:
                case 16:
                    return "🕓";
                case 5:
                case 17:
                    return "🕔";
                case 6:
                case 18:
                    return "🕕";
                case 7:
                case 19:
                    return "🕖";
                case 8:
                case 20:
                    return "🕗";
                case 9:
                case 21:
                    return "🕘";
                case 10:
                case 22:
                    return "🕙";
                case 11:
                case 23:
                    return "🕚";
            }

            return "";
        }
    
        public static DateTime GenerateDateBetweenRange(DateTime startDate, DateTime endDate)
        {            
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, 0, random.Next(0, (int)timeSpan.TotalSeconds));
            DateTime newDate = startDate + newSpan;

            return newDate;
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }
    }
}