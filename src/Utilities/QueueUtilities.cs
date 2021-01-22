using System;
using GearstashBot.Data;
using GearstashBot.Models;

namespace GearstashBot.Utilities
{
    public class QueueUtlities
    {
        public static double CalculateQueueApproxDays(int queueAmount = -1)
        {
            if(queueAmount == -1)
            {
                queueAmount = QueueData.CountQueuedQueueItems();
            }

            return TimeSpan
                .FromMilliseconds(
                    queueAmount*AppSettings.Config_PostInterval)
                .TotalDays; // BUG: This breaks after a large amount of days and starts showing a negative value
        }
    }
}