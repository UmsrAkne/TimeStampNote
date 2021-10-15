namespace TimeStampNote.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using Prism.Commands;
    using Prism.Mvvm;
    using TimeStampNote.Models;
    using TimeStampNote.Views;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private DelegateCommand addCommentCommand;
        private DelegateCommand<string> editCommentCommand;
        private DelegateCommand addGroupCommand;
        private DelegateCommand executeCommandCommand;
        private DelegateCommand getCommentCommand;
        private DelegateCommand reloadGroupNamesCommand;
        private DelegateCommand<string> toggleVisibilityCommand;
        private DelegateCommand toLigthThemeCommand;
        private DelegateCommand toDarkThemeCommand;

        private string commandText = string.Empty;

        public MainWindowViewModel()
        {
            ReloadGroupNamesCommand.Execute();
            GetCommentCommand.Execute();
        }

        public UIColors UIColors { get; } = new UIColors(Theme.Light);

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

        public ColumnVisibility ColumnVisibility { get; private set; } = new ColumnVisibility();

        public DelegateCommand AddCommentCommand => addCommentCommand ?? (addCommentCommand = new DelegateCommand(() =>
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.Text = Reader.OpenEditor($"{comment.SubID}.txt");
            comment.OrderNumber = DBHelper.GetNextOrderNumberInGroup();
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

        public DelegateCommand<string> EditCommentCommand => editCommentCommand ?? (editCommentCommand = new DelegateCommand<string>((subID) =>
        {
            var comment = DBHelper.GetLatastCommentFromSubID(subID);
            if (comment != null)
            {
                var updatedText = Reader.OpenEditor($"{comment.SubID}.txt", comment.Text);
                if (comment.Text != updatedText)
                {
                    comment.IsLatest = false;
                    DBHelper.Update(comment);
                    DBHelper.Insert(new Comment()
                    {
                        ID = DBHelper.GetMaxInColumn("comments", nameof(Comment.ID)) + 1,
                        SubID = comment.SubID,
                        OrderNumber = comment.OrderNumber,
                        Text = updatedText,
                        PostedDate = DateTime.Now,
                        GroupName = DBHelper.CurrentGroupName,
                        IsLatest = true
                    });
                }
            }
        }));

        public DelegateCommand ExecuteCommandCommand => executeCommandCommand ?? (executeCommandCommand = new DelegateCommand(() =>
        {
            var regOption = RegexOptions.IgnoreCase;

            if (Regex.IsMatch(CommandText, "^(a|add)( |$)", regOption))
            {
                AddCommentCommand.Execute();
            }

            if (Regex.IsMatch(CommandText, "^add-?group ", regOption))
            {
                AddGroupCommand.Execute();
            }

            if (Regex.IsMatch(CommandText, "^(e|edit) .+", regOption))
            {
                EditCommentCommand.Execute(Regex.Matches(CommandText, "^(e|edit) (.*)", regOption)[0].Groups[2].Value);
            }

            if (Regex.IsMatch(CommandText, "^(v|view) .+", regOption))
            {
                string subCommand = Regex.Match(CommandText, "^(v|view) (.*)$", regOption).Groups[2].Value.ToLower();
                ToggleVisibilityCommand.Execute(subCommand);
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

        public DelegateCommand<string> ToggleVisibilityCommand => toggleVisibilityCommand ?? (toggleVisibilityCommand = new DelegateCommand<string>((string param) =>
        {
            switch (param)
            {
                case "id":
                    ColumnVisibility.IDColumn = ColumnVisibility.ToggleVisibleAndCollapsed(ColumnVisibility.IDColumn);
                    break;

                case "subid":
                    ColumnVisibility.SubIDColumn = ColumnVisibility.ToggleVisibleAndCollapsed(ColumnVisibility.SubIDColumn);
                    break;

                case "date":
                    ColumnVisibility.DateColumn = ColumnVisibility.ToggleVisibleAndCollapsed(ColumnVisibility.DateColumn);
                    break;
            }
        }));

        public DelegateCommand ToLightThemeCommand
        {
            get => toLigthThemeCommand ?? (toLigthThemeCommand = new DelegateCommand(() => UIColors.Theme = Theme.Light));
        }

        public DelegateCommand ToDarkThemeCommand
        {
            get => toDarkThemeCommand ?? (toDarkThemeCommand = new DelegateCommand(() => UIColors.Theme = Theme.Dark));
        }
    }
}
