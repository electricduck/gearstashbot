using System;
using System.Collections.Generic;
using System.Linq;
using StashBot.Models;

namespace StashBot.Data
{
    public class AuthorData
    {
        public static Author CreateAuthor(long telegramId)
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);

                if (author == null)
                {
                    author = new Author
                    {
                        TelegramId = telegramId
                    };

                    db.Authors.Add(author);
                    db.SaveChanges();
                }

                return author;
            }
        }

        public static int CountAuthors()
        {
            using (var db = new StashBotDbContext())
            {
                return db.Authors
                    .ToList()
                    .Count();
            }

        }

        public static void DeleteAuthor(long telegramId)
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);

                if (author != null)
                {
                    db.Authors.Remove(author);
                    db.SaveChanges();
                }
            }
        }

        public static Author GetAuthor(long telegramId, bool createIfNotExist = true)
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);

                if (createIfNotExist)
                {
                    if (author == null)
                    {
                        author = new Author
                        {
                            TelegramId = telegramId
                        };

                        db.Authors.Add(author);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.SaveChanges();
                    }
                }

                return author;
            }
        }

        public static bool CanAuthorDeleteOthers(long telegramId)
        {
            Author author = GetAuthor(telegramId, false);

            if (author == null)
            {
                return false;
            }
            else
            {
                return author.CanDeleteOthers;
            }
        }

        public static bool CanAuthorFlushQueue(long telegramId)
        {
            Author author = GetAuthor(telegramId, false);

            if (author == null)
            {
                return false;
            }
            else
            {
                return author.CanFlushQueue;
            }
        }

        public static bool CanAuthorManageAuthors(long telegramId)
        {
            Author author = GetAuthor(telegramId, false);

            if (author == null)
            {
                return false;
            }
            else
            {
                return author.CanManageAuthors;
            }
        }

        public static bool CanAuthorQueue(long telegramId)
        {
            Author author = GetAuthor(telegramId, false);

            if (author == null)
            {
                return false;
            }
            else
            {
                return author.CanQueue;
            }
        }

        public static void SetAuthorDeleteOthersPermission(
            long telegramId,
            bool canDeleteOthers
        )
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);
                author.CanDeleteOthers = canDeleteOthers;
                db.SaveChanges();
            }
        }

        public static void SetAuthorFlushQueuePermission(
            long telegramId,
            bool canFlushQueue
        )
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);
                author.CanFlushQueue = canFlushQueue;
                db.SaveChanges();
            }
        }

        public static void SetAuthorQueuePermission(
            long telegramId,
            bool canQueue
        )
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);
                author.CanQueue = canQueue;
                db.SaveChanges();
            }
        }

        public static void SetAuthorManageAuthorsPermission(
            long telegramId,
            bool canManageAuthors
        )
        {
            using (var db = new StashBotDbContext())
            {
                Author author = db.Authors
                    .FirstOrDefault(a => a.TelegramId == telegramId);
                author.CanManageAuthors = canManageAuthors;
                db.SaveChanges();
            }
        }
    }
}