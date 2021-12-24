namespace TimeStampNote.ViewModels.Tests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TimeStampNote.Models;

    [TestClass]
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void MainWindowViewModelTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void SearchCommandTest()
        {
            var mv = new MainWindowViewModel();
            mv.Comments.Clear();

            mv.Comments.Add(new Comment() { Text = "testa" });
            mv.Comments.Add(new Comment() { Text = "testab" });
            mv.Comments.Add(new Comment() { Text = "testabc" });
            mv.Comments.Add(new Comment() { Text = "testabcd" });

            mv.SearchCommand.Execute("testabc");

            Assert.IsFalse(mv.Comments[0].IsMatch);
            Assert.IsFalse(mv.Comments[1].IsMatch);

            Assert.IsTrue(mv.Comments[2].IsMatch);
            Assert.IsTrue(mv.Comments[3].IsMatch);
        }
    }
}