using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.Controls
{
    /// <summary>
    /// Interaction logic for ButtonCornering.xaml
    /// </summary>
    public partial class ButtonCornering : UserControl
    {
        private Brush mainColor = Application.Current.Resources["Main"] as Brush;
        private Brush secondColor = Application.Current.Resources["MainSecond"] as Brush;

        public ButtonCornering()
        {
            InitializeComponent();
            MainBorder_LostFocus(null, null);
        }

        #region Border change color on mouse enter and leave
        private void MainBorder_GotFocus(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Background = secondColor;
        }

        private void MainBorder_LostFocus(object sender, RoutedEventArgs e)
        {
            this.MainBorder.Background = mainColor;
        }
        #endregion

        #region colors setters

        /// <summary>
        /// First color property
        /// </summary>
        public Brush FirstColor
        {
            set
            {
                SetValue(mainColorProperty, value);
                this.MainBorder.Background = value;
            }
        }
        public static readonly DependencyProperty mainColorProperty =
            DependencyProperty.Register("FirstColor", typeof(Brush), typeof(ButtonCornering),
                new PropertyMetadata(Application.Current.Resources["Main"] as Brush, new PropertyChangedCallback(OnChangeFirstColor)));

        private static void OnChangeFirstColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ButtonCornering).OnChangeFirstColor(e);
        }

        private void OnChangeFirstColor(DependencyPropertyChangedEventArgs e)
        {
            this.mainColor = e.NewValue as Brush;
            this.MainBorder.Background = e.NewValue as Brush;
        }

        /// <summary>
        /// Second color property
        /// </summary>
        public Brush SecondColor
        {
            set
            {
                secondColor = value;
                SetValue(secondColorProperty, value);
            }
        }
        public static readonly DependencyProperty secondColorProperty =
            DependencyProperty.Register("SecondColor", typeof(Brush), typeof(ButtonCornering),
                new PropertyMetadata(Application.Current.Resources["MainSecond"] as Brush, new PropertyChangedCallback(OnChangeSecondColor)));

        private static void OnChangeSecondColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ButtonCornering).OnChangeSecondColor(e);
        }

        private void OnChangeSecondColor(DependencyPropertyChangedEventArgs e)
        {
            this.secondColor = e.NewValue as Brush;
        }


        #endregion

        #region Button setings
        public string ButtonText
        {
            set
            {
                this.MainButton.Content = value;
            }
        }
        public double ButtonFontSize
        {
            set
            {
                this.MainButton.FontSize = value;
            }
        }
        #region Disable/Enable button
        public static readonly DependencyProperty EnableChangeProperty =
            DependencyProperty.Register("IsEnabledButton", typeof(bool),
                typeof(ButtonCornering),
                new PropertyMetadata(true, new PropertyChangedCallback(OnChangeAbility)));
        private static void OnChangeAbility(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ButtonCornering).OnChangeAbility(e);
        }

        private void OnChangeAbility(DependencyPropertyChangedEventArgs e)
        {
            changeAbility = (bool)(e.NewValue as bool?);
        }
        public bool IsEnabledButton
        {
            set
            {
                SetValue(EnableChangeProperty, value);
            }
        }
        private bool changeAbility
        {
            set
            {
                this.MainButton.IsEnabled = value;
            }
        }

        private void MainButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Button)sender).IsEnabled)
            {
                this.MainBorder.Background = mainColor;
                this.MainBorder.MouseEnter += MainBorder_GotFocus;
                this.MainBorder.MouseLeave += MainBorder_LostFocus;
            }
            else
            {
                this.MainBorder.Background = Brushes.Red;
                this.MainBorder.MouseEnter -= MainBorder_GotFocus;
                this.MainBorder.MouseLeave -= MainBorder_LostFocus;
            }
        }
        #endregion

        #endregion

        #region Click event handle
        public event RoutedEventHandler Click
        {
            add{AddHandler(AddClickEvent,value); }
            remove{RemoveHandler(AddClickEvent,value); }
        }

        void RaiseClickEvent()
        {
            RaiseEvent(new RoutedEventArgs(AddClickEvent));
        }
        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseClickEvent();
        }
        public static readonly RoutedEvent AddClickEvent = EventManager.RegisterRoutedEvent("Click",RoutingStrategy.Bubble,typeof(RoutedEventHandler),typeof(ButtonCornering));


        #endregion

    }
}
