namespace TimeStampNote.Views
{
    using System.Windows.Media;
    using Prism.Mvvm;

    public class UIColors : BindableBase
    {
        private Theme theme;
        private SolidColorBrush backgroundColorBrush;
        private SolidColorBrush deepBackgroundColorBrush;
        private SolidColorBrush foregroundColorBrush;
        private SolidColorBrush borderColorBrush;

        public UIColors(Theme defaultTheme)
        {
            Theme = defaultTheme;
        }

        public Theme Theme
        {
            get => theme;
            set
            {
                theme = value;
                if (theme == Theme.Dark)
                {
                    BackgroundColorBrush = new SolidColorBrush() { Color = Color.FromRgb(0x44, 0x44, 0x44) };
                    DeepBackgroundColorBrush = new SolidColorBrush() { Color = Color.FromRgb(0x33, 0x33, 0x33) };
                    ForegroundColorBrush = new SolidColorBrush() { Color = Colors.WhiteSmoke };
                    BorderColorBrush = new SolidColorBrush() { Color = Colors.DarkGray };
                }

                if (theme == Theme.Light)
                {
                    BackgroundColorBrush = new SolidColorBrush() { Color = Colors.WhiteSmoke };
                    DeepBackgroundColorBrush = new SolidColorBrush() { Color = Colors.White };
                    ForegroundColorBrush = new SolidColorBrush() { Color = Colors.Black };
                    BorderColorBrush = new SolidColorBrush() { Color = Colors.Gray };
                }
            }
        }

        public SolidColorBrush BackgroundColorBrush
        {
            get => backgroundColorBrush;
            private set => SetProperty(ref backgroundColorBrush, value);
        }

        public SolidColorBrush DeepBackgroundColorBrush
        {
            get => deepBackgroundColorBrush;
            private set => SetProperty(ref deepBackgroundColorBrush, value);
        }

        public SolidColorBrush ForegroundColorBrush
        {
            get => foregroundColorBrush;
            private set => SetProperty(ref foregroundColorBrush, value);
        }

        public SolidColorBrush BorderColorBrush
        {
            get => borderColorBrush;
            private set => SetProperty(ref borderColorBrush, value);
        }
    }
}
