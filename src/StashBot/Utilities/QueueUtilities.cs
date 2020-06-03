using System;
using StashBot.Models;

namespace StashBot.Utilities
{
    public class QueueUtilities
    {
        public static int GetSleepTime(int postsInQueue)
        {
            int day = 86400000;
            int maxPostsPerDay = AppSettings.Config_MaxPosts;
            int maxPosts = 0;

            if(postsInQueue == 0)
            {
                maxPosts = maxPostsPerDay;
            } else {
                maxPosts = (postsInQueue >= maxPostsPerDay) ? maxPostsPerDay : postsInQueue;
            }

            int sleepTime = day / maxPosts;

            return sleepTime;
        }
    }
}