namespace TimeStampNote.Models
{
    using Prism.Mvvm;

    public class OrderSetting : BindableBase
    {
        private bool reversing;
        private string sortColumnName = "index";

        public string SortColumnName
        {
            get => sortColumnName;
            set => SetProperty(ref sortColumnName, value == "date" || value == "id" ? value : "index");
        }

        public bool Reversing { get => reversing; set => SetProperty(ref reversing, value); }
    }
}
