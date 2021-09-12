namespace TimeStampNote.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using Prism.Commands;
    using Prism.Mvvm;
    using TimeStampNote.Models;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private DelegateCommand addCommentCommand;
        private DelegateCommand executeCommandCommand;
        private DelegateCommand getCommentCommand;
        private string commandText = string.Empty;

        public MainWindowViewModel()
        {
            GetCommentCommand.Execute();
        }

        public TextReader Reader { private get; set; } = new TextReader();

        public DBHelper DBHelper { get; } = new DBHelper("memoDB", "comments");

        public ObservableCollection<Comment> Comments { get; private set; } = new ObservableCollection<Comment>();

        public string CommandText
        {
            get => commandText;
            set => SetProperty(ref commandText, value);
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public DelegateCommand AddCommentCommand => addCommentCommand ?? (addCommentCommand = new DelegateCommand(() =>
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.Text = Reader.OpenEditor($"{comment.SubID}.txt");
            comment.PostedDate = DateTime.Now;
            comment.ID = DBHelper.GetMaxInColumn("comments", nameof(Comment.ID)) + 1;
            comment.GroupName = DBHelper.CurrentGroupName;
            comment.IsLatest = true;

            DBHelper.Insert(comment);
        }));

        public DelegateCommand ExecuteCommandCommand => executeCommandCommand ?? (executeCommandCommand = new DelegateCommand(() =>
        {
            if (Regex.IsMatch(CommandText, "^add ", RegexOptions.IgnoreCase))
            {
                AddCommentCommand.Execute();
            }
        }));

        public DelegateCommand GetCommentCommand => getCommentCommand ?? (getCommentCommand = new DelegateCommand(() =>
        {
            Comments.Clear();
            Comments.AddRange(DBHelper.GetAllComment());
        }));
    }
}
