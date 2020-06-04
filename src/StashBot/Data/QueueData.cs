using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StashBot.Models;

namespace StashBot.Data
{
    public class QueueData
    {
        public static void AddQueueItem(
            QueueItem item,
            TelegramUser user
        )
        {
            using (var db = new StashBotDbContext())
            {
                item.QueuedAt = DateTime.UtcNow;
                item.Status = QueueItem.QueueStatus.Queued;
                item.Author = db.Authors
                    .Where(a => a.TelegramId == user.Id)
                    .FirstOrDefault();

                db.Queue.Add(item);
                db.SaveChanges();
            }
        }

        public static int CountQueuedQueueItems()
        {
            using (var db = new StashBotDbContext())
            {
                return db.Queue
                    .Where(q => q.Status == QueueItem.QueueStatus.Queued)
                    .ToList()
                    .Count();
            }
        }

        public static void DeleteQueueItem(
            int id,
            bool realDelete
        )
        {
            using (var db = new StashBotDbContext())
            {
                var item = db.Queue
                    .FirstOrDefault(q => q.Id == id);

                if (item != null)
                {
                    if (realDelete)
                    {
                        db.Remove(item);
                    }
                    else
                    {
                        item.DeletedAt = DateTime.UtcNow;
                        item.Status = QueueItem.QueueStatus.Deleted;
                    }
                }

                db.SaveChanges();
            }
        }

        public static void DeleteRemovedQueueItems()
        {
            using (var db = new StashBotDbContext())
            {
                db.Queue
                    .RemoveRange(
                        db.Queue.Where(q => q.Status == QueueItem.QueueStatus.Deleted)
                    );
                db.SaveChanges();
            }
        }

        public static QueueItem GetLatestPostedQueueItem()
        {
            using (var db = new StashBotDbContext())
            {
                return db.Queue
                    .Include(q => q.Author)
                    .Where(q => q.Status == QueueItem.QueueStatus.Posted)
                    .OrderByDescending(q => q.PostedAt)
                    .FirstOrDefault();
            }
        }

        public static QueueItem GetQueueItem(int id)
        {
            using (var db = new StashBotDbContext())
            {
                var item = db.Queue
                    .Include(q => q.Author)
                    .Where(q => q.Id == id)
                    .FirstOrDefault();

                return item;
            }
        }

        public static QueueItem GetQueueItemBySourceUrl(string sourceUrl)
        {
            using (var db = new StashBotDbContext())
            {
                var item = db.Queue
                    .Include(q => q.Author)
                    .Where(q => q.SourceUrl == sourceUrl)
                    .FirstOrDefault();

                return item;
            }
        }

        public static QueueItem GetSoonestQueuedQueueItem()
        {
            using (var db = new StashBotDbContext())
            {
                return db.Queue
                    .Include(q => q.Author)
                    .Where(q => q.Status == QueueItem.QueueStatus.Queued)
                    .OrderBy(q => q.QueuedAt)
                    .FirstOrDefault();
            }
        }

        public static List<QueueItem> ListQueueItems()
        {
            using (var db = new StashBotDbContext())
            {
                var items = db.Queue
                    .Include(q => q.Author)
                    .OrderBy(q => q.QueuedAt)
                    .ToList();

                return items;
            }
        }

        public static List<QueueItem> ListQueuedQueueItems()
        {
            using (var db = new StashBotDbContext())
            {
                var items = db.Queue
                    .Include(q => q.Author)
                    .Where(q => q.Status == QueueItem.QueueStatus.Queued)
                    .OrderBy(q => q.QueuedAt)
                    .ToList();

                return items;
            }
        }

        public static void SetQueueItemAsPosted(
            int id,
            DateTime postedAt
        ) {
            using (var db = new StashBotDbContext())
            {
                var item = db.Queue
                    .Where(q => q.Id == id)
                    .FirstOrDefault();

                item.PostedAt = postedAt;
                item.Status = QueueItem.QueueStatus.Posted;
                db.SaveChanges();
            }
        }
    }
}