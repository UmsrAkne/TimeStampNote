namespace TimeStampNote.Models
{
    using System;

    public class Comment
    {
        private static Random random = new Random();

        /// <summary>
        /// このオブジェクトを識別するユニークな ID です。
        /// デフォルト状態では "-1" が割り当てられており、この値が、ID 未割り当ての状態を表します。
        /// </summary>
        public long ID { get; set; } = -1;

        /// <summary>
        /// このオブジェクトを識別するユニークな ID (文字列）です。
        /// </summary>
        public string SubID { get; set; }

        public string Text { get; set; }

        public DateTime PostedDate { get; set; }

        public bool IsLatest { get; set; }

        public string GroupName { get; set; }

        public long OrderNumber { get; set; }

        /// <summary>
        /// SubID 用の文字列を生成し、SubID にセットします。
        /// </summary>
        public void GenerateSubID()
        {
            var str = "abcdefghijklmnopqrstuvwxyz";

            SubID = string.Empty;
            for (var i = 0; i < 16; i++)
            {
                SubID += str.Substring(random.Next(0, str.Length), 1);
            }
        }
    }
}
