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
        private string title = string.Empty;
        private string groupName = "defaultGroup";
        private DelegateCommand addCommentCommand;
        private DelegateCommand<string> editCommentCommand;
        private DelegateCommand<string> deleteCommentCommand;
        private DelegateCommand addGroupCommand;
        private DelegateCommand executeCommandCommand;
        private DelegateCommand getCommentCommand;
        private DelegateCommand reloadGroupNamesCommand;
        private DelegateCommand reverseOrderCommand;
        private DelegateCommand<string> searchCommand;
        private DelegateCommand<string> sortCommand;
        private DelegateCommand<string> toggleVisibilityCommand;
        private DelegateCommand toLigthThemeCommand;
        private DelegateCommand toDarkThemeCommand;
        private DelegateCommand showSelectionCommentCommand;
        private DelegateCommand exitCommand;
        private DelegateCommand outputCommand;

        private string commandText = string.Empty;
        private string statusBarText;
        private Logger logger = new Logger();
        private CommentOutputter outputter = new CommentOutputter();

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

            if (!string.IsNullOrEmpty(Properties.Settings.Default.GroupName))
            {
                GroupName = Properties.Settings.Default.GroupName;
            }

            ReloadGroupNamesCommand.Execute();
            GetCommentCommand.Execute();
            UIColors.Theme = (Theme)Enum.ToObject(typeof(Theme), Properties.Settings.Default.Theme);
            Title = GroupName;
        }

        public UIColors UIColors { get; } = new UIColors(Theme.Light);

        public TextReader Reader { private get; set; } = new TextReader();

        public CommentDbContext DbContext { get; } = new CommentDbContext();

        public ObservableCollection<Comment> Comments { get; private set; } = new ObservableCollection<Comment>();

        public ObservableCollection<Comment> SelectedComments { get; private set; } = new ObservableCollection<Comment>();

        public ObservableCollection<string> GroupNames { get; private set; } = new ObservableCollection<string>();

        public OrderSetting OrderSetting { get; set; } = new OrderSetting();

        public string CommandText { get => commandText; set => SetProperty(ref commandText, value); }

        public string StatusBarText { get => statusBarText; set => SetProperty(ref statusBarText, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string GroupName
        {
            get => groupName;
            set
            {
                SetProperty(ref groupName, value);
                Title = value != null ? value : string.Empty;
                Properties.Settings.Default.GroupName = value != null ? value : string.Empty;
                Properties.Settings.Default.Save();
            }
        }

        public ColumnVisibility ColumnVisibility { get; private set; } = new ColumnVisibility();

        public DelegateCommand AddCommentCommand => addCommentCommand ?? (addCommentCommand = new DelegateCommand(() =>
        {
            var comment = GenerateComment(string.Empty);
            comment.Text = Reader.OpenEditor($"{comment.SubID}.txt");

            DbContext.Insert(new List<Comment>() { comment });
            logger.AddCommentLog(comment);
        }));

        public DelegateCommand AddGroupCommand => addGroupCommand ?? (addGroupCommand = new DelegateCommand(() =>
        {
            var comment = GenerateComment(string.Empty);
            comment.GroupName = Reader.OpenEditor($"Group_Name-{comment.SubID}.txt");

            if (!string.IsNullOrWhiteSpace(comment.GroupName))
            {
                DbContext.Insert(new List<Comment>() { comment });
                ReloadGroupNamesCommand.Execute();
                GroupName = comment.GroupName;
                GetCommentCommand.Execute();
            }
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

                    List<Comment> commentList = new List<Comment>()
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
                    };

                    DbContext.Insert(commentList);
                    logger.EditCommentLog(commentList.First());
                }

                GetCommentCommand.Execute();
            }
        }));

        public DelegateCommand<string> DeleteCommentCommand => deleteCommentCommand ?? (deleteCommentCommand = new DelegateCommand<string>((string subID) =>
        {
            List<Comment> comments = DbContext.GetComments(subID);
            if (comments.Count != 0)
            {
                comments.ForEach(comment => comment.Deleted = true);
                logger.DeleteCommentLog(comments.OrderByDescending(comment => comment.PostedDate).First());
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
            else if (Regex.IsMatch(CommandText, "^add-?group ?", regOption))
            {
                AddGroupCommand.Execute();
            }
            else if (Regex.IsMatch(CommandText, "^reverse-?order", regOption))
            {
                ReverseOrderCommand.Execute();
                CommandText = string.Empty;
                return;
            }
            else if (Regex.IsMatch(CommandText, @"^(set-?order) (\d+) (\d+)", regOption))
            {
                var matches = Regex.Matches(CommandText, @"^(set-?order) (\d+) (\d+)", regOption);
                var oldIndex = matches[0].Groups[2].Value;
                var newIndex = matches[0].Groups[3].Value;

                SetOrder(int.Parse(oldIndex), int.Parse(newIndex));
            }
            else if (Regex.IsMatch(CommandText, "^(e|edit) .+", regOption))
            {
                EditCommentCommand.Execute(Regex.Matches(CommandText, "^(e|edit) (.*)", regOption)[0].Groups[2].Value);
            }
            else if (Regex.IsMatch(CommandText, "^(d|del|delete) .+", regOption))
            {
                DeleteCommentCommand.Execute(Regex.Matches(CommandText, "^(d|del|delete) (.*)", regOption)[0].Groups[2].Value);
            }
            else if (Regex.IsMatch(CommandText, "^(v|view) .+", regOption))
            {
                string subCommand = Regex.Match(CommandText, "^(v|view) (.*)$", regOption).Groups[2].Value.ToLower();
                ToggleVisibilityCommand.Execute(subCommand);
            }
            else if (Regex.IsMatch(CommandText, "^sort .*", regOption))
            {
                string subCommand = Regex.Match(CommandText, "^(sort) (.*)$", regOption).Groups[2].Value.ToLower();
                OrderSetting.SortColumnName = subCommand;
            }
            else if (Regex.IsMatch(CommandText, "^(search) (.*)$", regOption))
            {
                string param = Regex.Match(CommandText, "^(search) (.*)$", regOption).Groups[2].Value.ToLower();
                SearchCommand.Execute(param);
            }
            else if (Regex.IsMatch(CommandText, "^exit ?"))
            {
                ExitCommand.Execute();
            }
            else
            {
                if (CommandText.Length > 0)
                {
                    var comment = GenerateComment(CommandText);
                    DbContext.Insert(new List<Comment>() { comment });
                    logger.AddCommentLog(comment);
                }
                else
                {
                    return;
                }
            }

            GetCommentCommand.Execute();
            CommandText = string.Empty;
        }));

        public DelegateCommand GetCommentCommand => getCommentCommand ?? (getCommentCommand = new DelegateCommand(() =>
        {
            Comments.Clear();
            var commentList = DbContext.GetGroupComments(GroupName, OrderSetting.SortColumnName);
            commentList = OrderSetting.Reversing ? commentList.AsEnumerable().Reverse().ToList() : commentList;
            Enumerable.Range(1, commentList.Count).ToList().ForEach(i => commentList[i - 1].LineNumber = i);
            Comments.AddRange(commentList);
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
                OrderSetting.Reversing = !OrderSetting.Reversing;
                GetCommentCommand.Execute();
                RaisePropertyChanged(nameof(Comments));
            }));
        }

        public DelegateCommand<string> SearchCommand
        {
            get => searchCommand ?? (searchCommand = new DelegateCommand<string>((string param) =>
            {
                Comments.ToList().ForEach(cm => cm.IsMatch = false);
                Comments.Where(cm => !string.IsNullOrEmpty(cm.Text) && cm.Text.Contains(param)).ToList()
                .ForEach(cm => cm.IsMatch = true);
            }));
        }

        public DelegateCommand<string> SortCommand
        {
            get => sortCommand ?? (sortCommand = new DelegateCommand<string>((string columnName) =>
            {
                OrderSetting.SortColumnName = columnName;
                GetCommentCommand.Execute();
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

        public DelegateCommand ExitCommand
        {
            get => exitCommand ?? (exitCommand = new DelegateCommand(() => System.Windows.Application.Current.Shutdown()));
        }

        public DelegateCommand OutputCommand
        {
            get => outputCommand ?? (outputCommand = new DelegateCommand(() =>
            {
                outputter.OutputToTextFile(DbContext.GetGroupComments(groupName));
            }));
        }

        private Comment GenerateComment(string text)
        {
            var comment = new Comment();
            comment.GenerateSubID();
            comment.Text = text;
            comment.OrderNumber = DbContext.GetNextOrderNumberInGroup(GroupName);
            comment.PostedDate = DateTime.Now;
            comment.ID = DbContext.GetMaxID() + 1;
            comment.GroupName = GroupName;
            comment.IsLatest = true;
            return comment;
        }

        private void SetOrder(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex && DbContext.GetCommentByOrderIndex(groupName, oldIndex).FirstOrDefault() == null)
            {
                return;
            }

            Comment targetComment = DbContext.GetCommentByOrderIndex(groupName, oldIndex).FirstOrDefault();
            List<Comment> updateComments;

            if (oldIndex > newIndex)
            {
                updateComments = DbContext.GetGroupComments(groupName)
                    .Where(c => c.OrderNumber < oldIndex && c.OrderNumber >= newIndex).ToList();
                updateComments.ForEach(c => c.OrderNumber++);
                DbContext.Update(updateComments);
            }
            else
            {
                updateComments = DbContext.GetGroupComments(groupName)
                    .Where(c => c.OrderNumber > oldIndex && c.OrderNumber <= newIndex).ToList();
                updateComments.ForEach(c => c.OrderNumber--);
                DbContext.Update(updateComments);
            }

            targetComment.OrderNumber = newIndex;
            DbContext.Update(new List<Comment>() { targetComment });
        }
    }
}
