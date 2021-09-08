namespace TimeStampNote.Models
{
using System.Collections.Generic;

    public interface IDBHelper
    {
        List<Comment> GetAllComment();

        void Insert(Comment comment);
    }
}
