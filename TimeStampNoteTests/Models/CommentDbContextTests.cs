namespace TimeStampNote.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                    new Comment() { ID = 2221, Text = "test1" },
                    new Comment() { ID = 2222, Text = "test2" },
                    new Comment() { ID = 2223, Text = "test3" }
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
                    new Comment() { ID = 2221, Text = "test1" },
                    new Comment() { ID = 2222, Text = "test2" },
                    new Comment() { ID = 2223, Text = "test3" }
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

        [TestMethod]
        public void GetLatastCommentFromSubIDTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

            using (var context = new CommentDbContext())
            {
                context.CreateDatabase();

                var comments = new List<Comment>()
                {
                    new Comment() { ID = 2221, Text = "test1", SubID = "abcdefg", PostedDate = new DateTime(100) },
                    new Comment() { ID = 2222, Text = "test2", SubID = "hijklmn", PostedDate = new DateTime(1200) },
                    new Comment() { ID = 2223, Text = "test3", SubID = "hijklmn", PostedDate = new DateTime(1100) },
                };

                context.Insert(comments);
                Assert.AreEqual(context.GetLatastCommentFromSubID("ijKl").Text, "test2");
            }

            File.Delete(databaseFileName);
        }

        [TestMethod]
        public void GetGroupNamesTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

            using (var context = new CommentDbContext())
            {
                context.CreateDatabase();

                var comments = new List<Comment>()
                {
                    new Comment() { ID = 2221, Text = "test1", GroupName = "g1" },
                    new Comment() { ID = 2222, Text = "test2", GroupName = "g2" },
                    new Comment() { ID = 2223, Text = "test3", GroupName = "g2" },
                    new Comment() { ID = 2224, Text = "test3", GroupName = "g3" },
                };

                context.Insert(comments);

                List<string> expected = new List<string> { "g1", "g2", "g3" };
                Assert.IsTrue(expected.SequenceEqual(context.GetGroupNames()));
            }

            File.Delete(databaseFileName);
        }

        [TestMethod]
        public void GetNextOrderNumberInGroupTest()
        {
            var cc = new CommentDbContext();
            string databaseFileName = cc.DBFileName;
            cc.Dispose();

            using (var context = new CommentDbContext())
            {
                context.CreateDatabase();

                var comments = new List<Comment>()
                {
                    new Comment() { ID = 2223, Text = "test1", OrderNumber = 1, GroupName = "g1" },
                    new Comment() { ID = 2224, Text = "test2", OrderNumber = 2, GroupName = "g1" },
                };

                context.Insert(comments);
                Assert.AreEqual(context.GetNextOrderNumberInGroup("g1"), 3);
            }

            File.Delete(databaseFileName);
        }
    }
}