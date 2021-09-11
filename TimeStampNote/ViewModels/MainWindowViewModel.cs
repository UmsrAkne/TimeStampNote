namespace TimeStampNote.ViewModels
{
    using System;
    using Prism.Commands;
    using Prism.Mvvm;
    using TimeStampNote.Models;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private DelegateCommand addCommentCommand;

        public MainWindowViewModel()
        {
        }

        public TextReader Reader { private get; set; } = new TextReader();

        public DBHelper DBHelper { get; } = new DBHelper("memoDB","comments");

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
            comment.IsLatest = true;

            DBHelper.Insert(comment);
        }));
    }
}
