namespace TimeStampNote.ViewModels
{
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

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public DelegateCommand AddCommentCommand => addCommentCommand ?? (addCommentCommand = new DelegateCommand(() =>
        {
            Reader.OpenEditor("temp.txt");
        }));
    }
}
