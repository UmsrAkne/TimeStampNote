namespace TimeStampNote.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;

    [TestClass]
    public class CommentDbContextTests
    {
        [TestMethod]
        public void InsertTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

            if (File.Exists(databaseFileName))
            {
                File.Delete(databaseFileName);
            }

            using (var context = new CommentDbContext())
            {
                context.CreateDatabase();

                var comments = new List<Comment>()
                {
                    new Comment(){ ID = 2221, Text = "test1"},
                    new Comment(){ ID = 2222 ,Text = "test2"},
                    new Comment(){ ID = 2223 ,Text = "test3"}
                };

                context.Insert(comments);
                databaseFileName = context.DBFileName;
            }

            File.Delete(databaseFileName);
        }
    }
}