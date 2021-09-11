﻿namespace TimeStampNote.ViewModels
{
    using System;
    using System.Text.RegularExpressions;
    using Prism.Commands;
    using Prism.Mvvm;
    using TimeStampNote.Models;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private DelegateCommand addCommentCommand;
        private DelegateCommand executeCommandCommand;
        private string commandText = string.Empty;

        public MainWindowViewModel()
        {
        }

        public TextReader Reader { private get; set; } = new TextReader();

        public DBHelper DBHelper { get; } = new DBHelper("memoDB", "comments");

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
    }
}
