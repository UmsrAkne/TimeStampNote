namespace TimeStampNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Logger
    {
        public string AddCommentLog(Comment comment)
        {
            return $"{DateTime.Now} {comment.SubID} を追加しました ----- \n {comment.Text} \n";
        }
    }
}
