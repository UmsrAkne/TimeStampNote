namespace TimeStampNote.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TimeStampNote.Models;

    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void AddCommentLogText()
        {
            Logger logger = new Logger();

            Comment comment = new Comment();
            comment.GenerateSubID();
            comment.Text = "testText";
            System.Diagnostics.Debug.WriteLine(logger.AddCommentLog(comment));
        }
    }
}