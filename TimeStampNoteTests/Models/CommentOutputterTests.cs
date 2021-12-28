namespace TimeStampNote.Models.Tests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommentOutputterTests
    {
        [TestMethod]
        public void FormatTextTest()
        {
            var outputter = new CommentOutputter();

            var comments = new List<Comment>()
            {
                new Comment() { ID = 1, Text = "test1", IsLatest = true, SubID = "aaaaa" },
                new Comment() { ID = 2, Text = "test2", IsLatest = true, SubID = "bbbbb" },
                new Comment() { ID = 3, Text = "test3", IsLatest = true, SubID = "ccccc" }
            };

            System.Diagnostics.Debug.WriteLine(outputter.FormatText(comments));
        }
    }
}