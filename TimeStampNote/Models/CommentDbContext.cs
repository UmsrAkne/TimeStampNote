namespace TimeStampNote.Models
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
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
    }
}
