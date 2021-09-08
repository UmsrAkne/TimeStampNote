namespace TimeStampNote.Models
{
    using System;

    public class Comment
    {
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
    }
}
