namespace TimeStampNote.Models
{
    using Prism.Mvvm;

    public class ColumnVisibility : BindableBase
    {
        private bool idColumn = true;
        private bool dateColumn = true;
        private bool orderNumberColumn = true;

        public bool IDColumn
        {
            get => idColumn;
            set => SetProperty(ref idColumn, value);
        }

        public bool DateColumn
        {
            get => dateColumn;
            set => SetProperty(ref dateColumn, value);
        }

        public bool OrderNumberColumn
        {
            get => orderNumberColumn;
            set => SetProperty(ref orderNumberColumn, value);
        }
    }
}
