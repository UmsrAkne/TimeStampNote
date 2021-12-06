namespace TimeStampNote.Models
{
    using System;

    public class Logger
    {
        public string AddCommentLog(Comment comment)
        {
            return Output(BuildMessage(comment, "を追加しました -----"));
        }

        public string EditCommentLog(Comment comment)
        {
            return Output(BuildMessage(comment, "を編集しました -----"));
        }

        public string DeleteCommentLog(Comment comment)
        {
            return Output(BuildMessage(comment, "を削除しました -----"));
        }

        private string BuildMessage(Comment comment, string message)
        {
            return $"{DateTime.Now} {comment.SubID} {message} \n\"{comment.Text}\"\n\n";
        }

        private string Output(string msg)
        {
            System.IO.File.AppendAllText("log.txt", msg);
            return msg;
        }
    }
}
