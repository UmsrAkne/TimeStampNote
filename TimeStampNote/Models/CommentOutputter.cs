namespace TimeStampNote.Models
{
    using System.Collections.Generic;
    using System.IO;

    public class CommentOutputter
    {
        public void OutputToTextFile(List<Comment> comments)
        {
            using (StreamWriter writer = File.CreateText("comment_output.txt"))
            {
                writer.WriteLine(FormatText(comments));
            }
        }

        public string FormatText(List<Comment> comments)
        {
            string combinedText = string.Empty;
            comments.ForEach(comment =>
            {
                if (comment.IsLatest)
                {
                    combinedText += $"{comment.ID} / {comment.SubID.Substring(0, 5)}... {comment.PostedDate} {comment.Text}\n";
                }
            });

            return combinedText;
        }
    }
}
