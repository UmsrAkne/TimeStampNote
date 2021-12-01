namespace TimeStampNote.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Prism.Commands;
    using Prism.Mvvm;
    using TimeStampNote.Models;
    using TimeStampNote.Views;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private string groupName = "defaultGroup";
        private DelegateCommand addCommentCommand;
        private DelegateCommand<string> editCommentCommand;
        private DelegateCommand<string> deleteCommentCommand;
        private DelegateCommand addGroupCommand;
        private DelegateCommand executeCommandCommand;
        private DelegateCommand getCommentCommand;
        private DelegateCommand reloadGroupNamesCommand;
        private DelegateCommand reverseOrderCommand;
        private DelegateCommand<string> toggleVisibilityCommand;
        private DelegateCommand toLigthThemeCommand;
        private DelegateCommand toDarkThemeCommand;
        private DelegateCommand showSelectionCommentCommand;

        private string commandText = string.Empty;
        private string statusBarText;

        public MainWindowViewModel()
        {
            DbContext.CreateDatabase();

            if (DbContext.GetAll().Count == 0)
            {
                var comment = new Comment();
                comment.GenerateSubID();
                comment.PostedDate = DateTime.Now;
                comment.ID = DbContext.GetMaxID() + 1;
                comment.GroupName = groupName;
                comment.IsLatest = true;
                DbContext.Insert(new List<Comment>() { comment });
            }

            ReloadGroupNamesCommand.Execute();
            GetCommentCommand.Execute();
            UIColors.Theme = (Theme)Enum.ToObject(typeof(Theme), Properties.Settings.Default.Theme);
        }

        public UIColors UIColors { get; } = new UIColors(Theme.Light);

        public TextReader Reader { private get; set; } = new TextReader();

        public CommentDbContext DbContext { get; } = new CommentDbContext();

        public ObservableCollection<Comment> Comments { get; private set; } = new ObservableCollection<Comment>();

        public ObservableCollection<Comment> SelectedComments { get; private set; } = new ObservableCollection<Comment>();

        public ObservableCollection<string> GroupNames { get; private set; } = new ObservableCollection<string>();

        public string CommandText
        {
            get => commandText;
            set => SetProperty(ref commandText, value);
        }

        public string StatusBarText { get => statusBarText; set => SetProperty(ref statusBarText, value); }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string GroupName { get => groupName; set => SetProperty(ref groupName, value); }

        public ColumnVisibility ColumnVisibility { get; private set; } = new ColumnVisibility();

        public DelegateCommand AddCommentCommand => addCommentCommand ?? (addCommentCommand = new DelegateCommand(() =>
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.Text = Reader.OpenEditor($"{comment.SubID}.txt");
            comment.OrderNumber = DbContext.GetNextOrderNumberInGroup(GroupName);
            comment.PostedDate = DateTime.Now;
            comment.ID = DbContext.GetMaxID() + 1;
            comment.GroupName = GroupName;
            comment.IsLatest = true;

            DbContext.Insert(new List<Comment>() { comment });
        }));

        public DelegateCommand AddGroupCommand => addGroupCommand ?? (addGroupCommand = new DelegateCommand(() =>
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.PostedDate = DateTime.Now;
            comment.ID = DbContext.GetMaxID() + 1;
            comment.GroupName = Reader.OpenEditor($"Group_Name-{comment.SubID}.txt");
            comment.IsLatest = true;
            DbContext.Insert(new List<Comment>() { comment });

            ReloadGroupNamesCommand.Execute();
            GroupName = comment.GroupName;
            GetCommentCommand.Execute();
        }));

        public DelegateCommand<string> EditCommentCommand => editCommentCommand ?? (editCommentCommand = new DelegateCommand<string>((subID) =>
        {
            var comment = DbContext.GetLatastCommentFromSubID(subID);
            if (comment != null)
            {
                var updatedText = Reader.OpenEditor($"{comment.SubID}.txt", comment.Text);
                if (comment.Text != updatedText)
                {
                    comment.IsLatest = false;
                    DbContext.Update(comment);
                    DbContext.Insert(new List<Comment>()
                    {
                        new Comment()
                        {
                            ID = DbContext.GetMaxID() + 1,
                            SubID = comment.SubID,
                            OrderNumber = comment.OrderNumber,
                            Text = updatedText,
                            PostedDate = DateTime.Now,
                            GroupName = GroupName,
                            IsLatest = true
                        }
                    });
                }
            }
        }));

        public DelegateCommand<string> DeleteCommentCommand => deleteCommentCommand ?? (deleteCommentCommand = new DelegateCommand<string>((string subID) =>
        {
            List<Comment> comments = DbContext.GetComments(subID);
            if (comments.Count != 0)
            {
                comments.ForEach(comment => comment.Deleted = true);
                DbContext.Update(comments);
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

            if (Regex.IsMatch(CommandText, "^reverse-?order", regOption))
            {
                ReverseOrderCommand.Execute();
                CommandText = string.Empty;
                return;
            }

            if (Regex.IsMatch(CommandText, "^(e|edit) .+", regOption))
            {
                EditCommentCommand.Execute(Regex.Matches(CommandText, "^(e|edit) (.*)", regOption)[0].Groups[2].Value);
            }

            if (Regex.IsMatch(CommandText, "^(d|del|delete) .+", regOption))
            {
                DeleteCommentCommand.Execute(Regex.Matches(CommandText, "^(d|del|delete) (.*)", regOption)[0].Groups[2].Value);
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
            Comments.AddRange(DbContext.GetGroupComments(GroupName));
        }));

        public DelegateCommand ReloadGroupNamesCommand => reloadGroupNamesCommand ?? (reloadGroupNamesCommand = new DelegateCommand(() =>
        {
            GroupNames.Clear();
            GroupNames.AddRange(DbContext.GetGroupNames());
        }));

        public DelegateCommand ReverseOrderCommand
        {
            get => reverseOrderCommand ?? (reverseOrderCommand = new DelegateCommand(() =>
            {
                Comments = new ObservableCollection<Comment>(Comments.Reverse());
                RaisePropertyChanged(nameof(Comments));
            }));
        }

        public DelegateCommand<string> ToggleVisibilityCommand => toggleVisibilityCommand ?? (toggleVisibilityCommand = new DelegateCommand<string>((string param) =>
        {
            switch (param)
            {
                case "id":
                    ColumnVisibility.IDColumn = ColumnVisibility.ToggleVisibleAndCollapsed(ColumnVisibility.IDColumn);
                    break;

                case "order-number":
                    ColumnVisibility.OrderNumberColumn = ColumnVisibility.ToggleVisibleAndCollapsed(ColumnVisibility.OrderNumberColumn);
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
            get => toLigthThemeCommand ?? (toLigthThemeCommand = new DelegateCommand(() =>
            {
                UIColors.Theme = Theme.Light;
                Properties.Settings.Default.Theme = (int)Theme.Light;
                Properties.Settings.Default.Save();
            }));
        }

        public DelegateCommand ToDarkThemeCommand
        {
            get => toDarkThemeCommand ?? (toDarkThemeCommand = new DelegateCommand(() =>
            {
                UIColors.Theme = Theme.Dark;
                Properties.Settings.Default.Theme = (int)Theme.Dark;
                Properties.Settings.Default.Save();
            }));
        }

        public DelegateCommand ShowSelectionCommentCommand
        {
            get => showSelectionCommentCommand ?? (showSelectionCommentCommand = new DelegateCommand(() =>
            {
                var selections = Comments.Where(cm => cm.IsSelected);

                if (selections.Count() == 0)
                {
                    StatusBarText = string.Empty;
                }
                else if (selections.Count() == 1)
                {
                    StatusBarText = $"No. {selections.First().OrderNumber} ({selections.First().SubID}) を選択中";
                }
                else
                {
                    StatusBarText = $"{selections.Count()} 個のコメントを選択中";
                }
            }));
        }
    }
}
