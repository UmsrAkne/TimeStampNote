namespace TimeStampNote.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Comment
    {
        private static Random random = new Random();

        /// <summary>
        /// このオブジェクトを識別するユニークな ID です。
        /// デフォルト状態では "-1" が割り当てられており、この値が、ID 未割り当ての状態を表します。
        /// </summary>
        [Key]
        [Required]
        [Column("id")]
        public long ID { get; set; } = -1;

        /// <summary>
        /// このオブジェクトを識別するユニークな ID (文字列）です。
        /// </summary>
        [Column("sub_id")]
        public string SubID { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("posted_date")]
        public DateTime PostedDate { get; set; }

        [Column("is_latest")]
        public bool IsLatest { get; set; }

        [Column("group_name")]
        public string GroupName { get; set; }

        [Column("order_number")]
        public long OrderNumber { get; set; }

        [NotMapped]
        public bool IndexIsEven { get; set; }

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
