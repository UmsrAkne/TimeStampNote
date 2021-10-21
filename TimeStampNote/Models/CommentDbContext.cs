namespace TimeStampNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    public class CommentDbContext : DbContext
    {
        public string DBFileName => "Comments.sqlite";

        public DbSet<Comment> Comments { get; set; }

        public void Insert(List<Comment> comments)
        {
            comments.ForEach(c => Comments.Add(c));
            SaveChanges();
        }

        public void Update(Comment comment)
        {
            Comment target = Comments.Where(c => comment.ID == c.ID).First();
            target.Text = comment.Text;
            target.PostedDate = comment.PostedDate;
            target.GroupName = comment.GroupName;
            target.IsLatest = comment.IsLatest;
            SaveChanges();
        }

        public Comment GetLatastCommentFromSubID(string partOfSubID)
        {
            var list = Comments.Where(c => c.SubID.IndexOf(partOfSubID, StringComparison.OrdinalIgnoreCase) != -1)
                   .OrderByDescending(c => c.PostedDate);

            return list.Count() != 0 ? list.First() : null;
        }

        public List<Comment> GetGroupComments(string groupName)
        {
            return Comments.Where(c => c.GroupName == groupName && c.IsLatest)
                .OrderBy(c => c.OrderNumber).ToList();
        }

        public List<Comment> GetAll() => Comments.Select(comment => comment).ToList();

        public int GetRecordCount() => Comments.Select(c => c).Count();

        public List<string> GetGroupNames() => Comments.Select(c => c.GroupName).Distinct().ToList();

        public long GetNextOrderNumberInGroup(string groupName)
        {
            var numbers = Comments.Where(c => c.GroupName == groupName).Select(c => c.OrderNumber).OrderByDescending(number => number);
            return numbers.Count() != 0 ? numbers.First() + 1 : 0;
        }

        public long GetMaxID()
        {
            var ids = Comments.Select(c => c.ID).OrderByDescending(n => n);
            return ids.Count() != 0 ? ids.First() : 0;
        }

        public void CreateDatabase()
        {
            if (!File.Exists(DBFileName))
            {
                Database.EnsureCreated();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!File.Exists(DBFileName))
            {
                SQLiteConnection.CreateFile(DBFileName);
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = DBFileName }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }
    }
}
