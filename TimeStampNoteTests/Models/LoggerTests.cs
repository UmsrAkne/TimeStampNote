using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeStampNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeStampNote.Models.Tests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void addCommentLogTest()
        {
            Logger logger = new Logger();

            Comment comment = new Comment();
            comment.GenerateSubID();
            comment.Text = "testText";
            System.Diagnostics.Debug.WriteLine(logger.AddCommentLog(comment));
        }
    }
}