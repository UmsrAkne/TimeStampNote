namespace TimeStampNote.Models
{
    using System.Collections.Generic;

    public class CommentOutputter
    {
        public void OutputToTextFile()
        {
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
