namespace TimeStampNote.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;
    using Prism.Mvvm;

    public class DBHelper : BindableBase, IDBHelper
    {
        private string currentGroupName = "defaultGroup";

        public DBHelper(string databaseName, string tableName)
        {
            DatabaseName = databaseName;
            TableName = tableName;

            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);
            }

            var createTableSql = $"CREATE TABLE IF NOT EXISTS {TableName} (";
            createTableSql += $"{nameof(Comment.ID)} INTEGER PRIMARY KEY NOT NULL, ";
            createTableSql += $"{nameof(Comment.SubID)} TEXT , ";
            createTableSql += $"{nameof(Comment.Text)} TEXT , ";
            createTableSql += $"{nameof(Comment.PostedDate)} TEXT , ";
            createTableSql += $"{nameof(Comment.IsLatest)} TEXT , ";
            createTableSql += $"{nameof(Comment.GroupName)} TEXT ";
            createTableSql += $");";

            ExecuteNonQuery(createTableSql);
        }

        public string DatabaseName { get; private set; }

        public string TableName { get; private set; }

        public string CurrentGroupName { get => currentGroupName; set => SetProperty(ref currentGroupName, value); }

        public void ExecuteNonQuery(string commandText)
        {
            using (var conn = new SQLiteConnection("Data Source=" + DatabaseName))
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        /// <summary>
        /// パラメーターに指定したコマンドを実行し、取得したSQLiteDataReaderから情報を読み込む
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>内部で取得したSQLiteDataReaderの値をすべて詰め込んだオブジェクトを取得する</returns>
        public List<Dictionary<string, object>> Select(string commandText)
        {
            using (var conn = new SQLiteConnection("Data Source=" + DatabaseName))
            {
                using (var command = new SQLiteCommand(commandText, conn))
                {
                    conn.Open();
                    using (SQLiteDataReader sdr = command.ExecuteReader())
                    {
                        var dictionarys = new List<Dictionary<string, object>>();
                        while (sdr.Read())
                        {
                            var dic = new Dictionary<string, object>();
                            for (var i = 0; i < sdr.FieldCount; i++)
                            {
                                dic[sdr.GetName(i)] = sdr.GetValue(i);
                            }

                            dictionarys.Add(dic);
                        }

                        return dictionarys;
                    }
                }
            }
        }

        /// <summary>
        /// 指定テーブル、指定列内の最大値を取得します
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public long GetMaxInColumn(string tableName, string columnName)
        {
            if (GetRecordCount(TableName) == 0)
            {
                return 0;
            }

            var commandText = "SELECT MAX(" + columnName + ") FROM " + tableName;
            var dics = Select(commandText);
            return (long)dics[0]["MAX(" + columnName + ")"];
        }

        /// <summary>
        /// 指定したテーブルに入っている総レコード数を取得します
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public long GetRecordCount(string tableName)
        {
            var commandText = "SELECT id FROM " + tableName;
            var dics = Select(commandText);
            return dics.Count;
        }

        public List<Comment> GetAllComment()
        {
            var dics = Select($"SELECT * FROM {TableName};");
            var comments = new List<Comment>();

            dics.ForEach((Dictionary<string, object> d) => comments.Add(ToComment(d)));

            return comments;
        }

        public void Insert(Comment comment)
        {
            var commandText = $"INSERT INTO {TableName} " +
                $"({nameof(Comment.ID)}, " +
                $"{nameof(Comment.SubID)}, " +
                $"{nameof(Comment.PostedDate)}, " +
                $"{nameof(Comment.Text)}, " +
                $"{nameof(Comment.IsLatest)}, " +
                $"{nameof(Comment.GroupName)}) " +
                $"values " +
                $"({comment.ID}, " +
                $"'{comment.SubID}', " +
                $"'{comment.PostedDate}', " +
                $"'{comment.Text}', " +
                $"'{comment.IsLatest}', " +
                $"'{comment.GroupName}');";

            ExecuteNonQuery(commandText);
        }

        public List<string> GetGroupNames()
        {
            var dics = Select($"SELECT DISTINCT {nameof(Comment.GroupName)} FROM {TableName};");

            var names = new List<string>();
            dics.ForEach(d =>
            {
                names.Add((string)d[nameof(Comment.GroupName)]);
            });

            return names;
        }

        private Comment ToComment(Dictionary<string, object> dic) => new Comment()
        {
            ID = (long)dic[nameof(Comment.ID)],
            SubID = (string)dic[nameof(Comment.SubID)],
            PostedDate = DateTime.Parse((string)dic[nameof(Comment.PostedDate)]),
            GroupName = (string)dic[nameof(Comment.GroupName)],
            Text = (string)dic[nameof(Comment.Text)],
            IsLatest = Convert.ToBoolean(dic[nameof(Comment.IsLatest)])
        };
    }
}
