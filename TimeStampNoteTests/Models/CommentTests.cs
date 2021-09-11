namespace TimeStampNote.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TimeStampNote.Models;

    [TestClass]
    public class CommentTests
    {
        [TestMethod]
        public void GenerateSubIDTest()
        {
            var comment = new Comment();
            comment.GenerateSubID();
            Assert.AreEqual(comment.SubID.Length, 16);
        }
    }
}