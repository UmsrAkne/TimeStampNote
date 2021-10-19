
namespace TimeStampNote.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommentDbContextTests
    {
        [TestMethod]
        public void insertTest()
        {
            using (var context = new CommentDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}