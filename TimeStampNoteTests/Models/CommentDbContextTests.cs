namespace TimeStampNote.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [TestClass]
    public class CommentDbContextTests
    {
        [TestMethod]
        public void InsertTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

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

                var list = context.GetAll();
                Assert.AreEqual(list.Count, 3);
                Assert.AreEqual(list[0].ID, 2221);
            }

            File.Delete(databaseFileName);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

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

                var comment = context.GetAll().First();
                Assert.AreEqual(comment.Text, "test1");

                comment.Text = "test11";
                context.Update(comment);

                Assert.AreEqual(context.GetAll().First().Text, "test11");
            }

            File.Delete(databaseFileName);
        }
    }
}