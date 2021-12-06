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
            return buildMessage(comment, "を追加しました -----");
        }

        public string EditCommentLog(Comment comment)
        {
            return buildMessage(comment, "を編集しました -----");
        }

        public string DeleteCommentLog(Comment comment)
        {
            return buildMessage(comment, "を削除しました -----");
        }

        private string buildMessage(Comment comment, string message)
        {
            return $"{DateTime.Now} {comment.SubID} {message} \n Text =  \n\"{comment.Text}\"\n";
        }

    }
}
