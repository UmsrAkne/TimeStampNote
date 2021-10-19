namespace TimeStampNote.Models
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using System.Data.SQLite;

    public class CommentDbContext : DbContext
    {

        public DbSet<Comment> Comments { get; set; }

        public CommentDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SQLiteConnection.CreateFile("Comments.sqlite");
            var connectionString = new SqliteConnectionStringBuilder { DataSource = @"Comments.sqlite" }.ToString();
            optionsBuilder.UseSqlite(new SQLiteConnection(connectionString));
        }

        public void insert()
        {
        }
    }
}
