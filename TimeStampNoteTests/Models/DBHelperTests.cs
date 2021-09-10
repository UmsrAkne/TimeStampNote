using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeStampNote.Models;

namespace TimeStampNote.Models.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DBHelperTests
    {

        private readonly string dbName = "testDB";
        private readonly string tableName = "testTable";

        [TestMethod]
        public void DBHelperTest()
        {
            // db 生成のテスト

            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }

            DBHelper dbHelper = new DBHelper(dbName, tableName);

            Assert.IsTrue(File.Exists(dbName));
        }

        [TestMethod()]
        public void InsertTest()
        {
            if (File.Exists(dbName))
            {
                File.Delete(dbName);
            }

            DBHelper dbHelper = new DBHelper(dbName, tableName);

            var comment = new Comment()
            {
                ID = 2,
                SubID = "abc",
                PostedDate = new DateTime(0),
                Text = "testText",
                GroupName = "testGroup"
            };

            var comment2 = new Comment();

            dbHelper.Insert(comment);
            dbHelper.Insert(comment2);

            var dics = dbHelper.Select($"select * from {tableName} order by {nameof(Comment.ID)};");
            Assert.AreEqual(dics.Count, 2, "insert を２回実行したので Count == 2");

            long longID = 2;

            // DBに入力した要素を取り出して確認する。

            Assert.AreEqual(dics[1][nameof(Comment.ID)], longID);
            Assert.AreEqual(dics[1][nameof(Comment.SubID)], "abc");
            Assert.AreEqual(dics[1][nameof(Comment.Text)], "testText");
            Assert.AreEqual(dics[1][nameof(Comment.GroupName)], "testGroup");
            Assert.AreEqual(dics[1][nameof(Comment.PostedDate)], new DateTime(0).ToString());
        }
    }
}