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
        private DelegateCommand editCommentCommand;
        private DelegateCommand addGroupCommand;
        private DelegateCommand executeCommandCommand;
        private DelegateCommand getCommentCommand;
        private DelegateCommand reloadGroupNamesCommand;
        private string commandText = string.Empty;

        public MainWindowViewModel()
        {
            ReloadGroupNamesCommand.Execute();
            GetCommentCommand.Execute();
        }

        public TextReader Reader { private get; set; } = new TextReader();

        public DBHelper DBHelper { get; } = new DBHelper("memoDB", "comments");

        public ObservableCollection<Comment> Comments { get; private set; } = new ObservableCollection<Comment>();

        public ObservableCollection<string> GroupNames { get; private set; } = new ObservableCollection<string>();

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

        public DelegateCommand AddGroupCommand => addGroupCommand ?? (addGroupCommand = new DelegateCommand(() =>
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.PostedDate = DateTime.Now;
            comment.ID = DBHelper.GetMaxInColumn("comments", nameof(Comment.ID)) + 1;
            comment.GroupName = Reader.OpenEditor($"Group_Name-{comment.SubID}.txt");
            comment.IsLatest = true;
            DBHelper.Insert(comment);
        }));

        public DelegateCommand EditCommentCommand => editCommentCommand ?? (editCommentCommand = new DelegateCommand(() =>
        {
        }));

        public DelegateCommand ExecuteCommandCommand => executeCommandCommand ?? (executeCommandCommand = new DelegateCommand(() =>
        {
            var regOption = RegexOptions.IgnoreCase;

            if (Regex.IsMatch(CommandText, "^add ", regOption))
            {
                AddCommentCommand.Execute();
            }

            if (Regex.IsMatch(CommandText, "^add-?group ", regOption))
            {
                AddGroupCommand.Execute();
            }

            if (Regex.IsMatch(CommandText, "^edit .+", regOption))
            {
            }

            GetCommentCommand.Execute();
            CommandText = string.Empty;
        }));

        public DelegateCommand GetCommentCommand => getCommentCommand ?? (getCommentCommand = new DelegateCommand(() =>
        {
            Comments.Clear();
            Comments.AddRange(DBHelper.GetGroupComments());
        }));

        public DelegateCommand ReloadGroupNamesCommand => reloadGroupNamesCommand ?? (reloadGroupNamesCommand = new DelegateCommand(() =>
        {
            GroupNames.Clear();
            GroupNames.AddRange(DBHelper.GetGroupNames());
        }));
    }
}
