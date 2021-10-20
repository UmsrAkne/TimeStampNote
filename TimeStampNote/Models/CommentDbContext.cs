namespace TimeStampNote.Models
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;

    public class CommentDbContext : DbContext
    {
        public string DBFileName => "Comments.sqlite";

        public DbSet<Comment> Comments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!File.Exists(DBFileName))
            {
                SQLiteConnection.CreateFile(DBFileName);
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = DBFileName }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }

        public void CreateDatabase()
        {
            if (!File.Exists(DBFileName))
            {
                Database.EnsureCreated();
            }
        }

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
            return Comments.Where(c => c.SubID.IndexOf(partOfSubID, StringComparison.OrdinalIgnoreCase) != -1)
                   .OrderByDescending(c => c.PostedDate).First();
        }

        public List<Comment> GetGroupComments(string groupName) => Comments.Where(c => c.GroupName == groupName).ToList();

        public List<Comment> GetAll() => Comments.Select(comment => comment).ToList();

        public int GetRecordCount() => Comments.Select(c => c).Count();
    }
}
