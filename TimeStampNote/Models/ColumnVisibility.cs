namespace TimeStampNote.Models
{
    using System.Windows;
    using Prism.Mvvm;

    public class ColumnVisibility : BindableBase
    {
        private Visibility idColumn = Visibility.Collapsed;
        private Visibility subIDColumn = Visibility.Visible;
        private Visibility dateColumn = Visibility.Visible;
        private Visibility orderNumberColumn = Visibility.Visible;

        public Visibility IDColumn
        {
            get => idColumn;
            set => SetProperty(ref idColumn, value);
        }

        public Visibility SubIDColumn
        {
            get => subIDColumn;
            set => SetProperty(ref subIDColumn, value);
        }

        public Visibility DateColumn
        {
            get => dateColumn;
            set => SetProperty(ref dateColumn, value);
        }

        public Visibility OrderNumberColumn
        {
            get => orderNumberColumn;
            set => SetProperty(ref orderNumberColumn, value);
        }

        public Visibility ToggleVisibleAndCollapsed(Visibility currentVisibility) =>
            currentVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
}
