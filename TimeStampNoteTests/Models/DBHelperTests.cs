namespace TimeStampNote.Models.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TimeStampNote.Models;

    [TestClass]
    public class DBHelperTests
    {
        private readonly string databaseName = "testDB";
        private readonly string tableName = "testTable";

        [TestMethod]
        public void DBHelperTest()
        {
            /// db 生成のテスト

            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }

            DBHelper databaseHelper = new DBHelper(databaseName, tableName);

            Assert.IsTrue(File.Exists(databaseName));
        }

        [TestMethod]
        public void InsertTest()
        {
            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }

            DBHelper databaseHelper = new DBHelper(databaseName, tableName);

            var comment = new Comment()
            {
                ID = 2,
                SubID = "abc",
                PostedDate = new DateTime(0),
                Text = "testText",
                GroupName = "testGroup"
            };

            var comment2 = new Comment();

            databaseHelper.Insert(comment);
            databaseHelper.Insert(comment2);

            var dics = databaseHelper.Select($"select * from {tableName} order by {nameof(Comment.ID)};");
            Assert.AreEqual(dics.Count, 2, "insert を２回実行したので Count == 2");

            long longID = 2;

            /// DBに入力した要素を取り出して確認する。

            Assert.AreEqual(dics[1][nameof(Comment.ID)], longID);
            Assert.AreEqual(dics[1][nameof(Comment.SubID)], "abc");
            Assert.AreEqual(dics[1][nameof(Comment.Text)], "testText");
            Assert.AreEqual(dics[1][nameof(Comment.GroupName)], "testGroup");
            Assert.AreEqual(dics[1][nameof(Comment.PostedDate)], new DateTime(0).ToString());
        }

        [TestMethod]
        public void GetAllCommentTest()
        {
            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }

            DBHelper databaseHelper = new DBHelper(databaseName, tableName);

            var comment = new Comment()
            {
                ID = 2,
                SubID = "abc",
                PostedDate = new DateTime(0),
                Text = "testText",
                GroupName = "testGroup"
            };

            var comment2 = new Comment()
            {
                ID = 3,
                SubID = "def",
                PostedDate = new DateTime(1),
                Text = "testTextNext",
                GroupName = "testGroup"
            };

            databaseHelper.Insert(comment);
            databaseHelper.Insert(comment2);

            var comments = databaseHelper.GetAllComment();

            Assert.AreEqual(comments.Count, 2);
            Assert.AreEqual(comments[0].Text, "testText");
            Assert.AreEqual(comments[0].GroupName, "testGroup");
            Assert.AreEqual(comments[0].PostedDate, new DateTime(0));
        }

        [TestMethod]
        public void GetGroupNamesTest()
        {
            if (File.Exists(databaseName))
            {
                File.Delete(databaseName);
            }

            DBHelper databaseHelper = new DBHelper(databaseName, tableName);

            var comment = new Comment()
            {
                ID = 2,
                SubID = "abc",
                GroupName = "testGroup"
            };

            var comment2 = new Comment()
            {
                ID = 3,
                SubID = "def",
                GroupName = "testGroup2"
            };

            var comment3 = new Comment()
            {
                ID = 4,
                SubID = "def",
                GroupName = "testGroup2"
            };

            databaseHelper.Insert(comment);
            databaseHelper.Insert(comment2);

            var groupNames = databaseHelper.GetGroupNames();

            Assert.AreEqual(groupNames[0], "testGroup");
            Assert.AreEqual(groupNames[1], "testGroup2");
            Assert.AreEqual(groupNames.Count, 2);
        }
    }
}