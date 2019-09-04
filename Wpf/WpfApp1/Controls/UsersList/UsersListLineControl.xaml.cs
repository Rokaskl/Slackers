using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1.Controls.UsersList
{
    /// <summary>
    /// Interaction logic for FriendsListLineControl.xaml
    /// </summary>
    public partial class UsersListLineControl : UserControl
    {
        private ImageBrush plus;
        private ImageBrush minus;

        public UsersListLineControl()
        {
            InitializeComponent();
        }

        private void BioPanel_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.bio_panel.Visibility == Visibility.Hidden || this.bio_panel.Visibility == Visibility.Collapsed)
            {
                this.bio_panel.Visibility = Visibility.Visible;
                this.bio_panel.Height = 120;
                this.content_grid.RowDefinitions[1].Height = new GridLength(120);
                if (this.minus == null)
                {
                    ImageBrush img_brush = new ImageBrush();
                    img_brush.ImageSource = new BitmapImage(new System.Uri("pack://application:,,,/WpfApp1;component/Images/minus_blue_remove.png"));
                    this.minus = img_brush;
                }
                this.BioPanel_btn.Background = minus;
            }
            else
            {
                this.bio_panel.Visibility = Visibility.Hidden;
                this.content_grid.RowDefinitions[1].Height = new GridLength(0);
                this.bio_panel.Height = 0;
                if(this.plus == null)
                {
                    ImageBrush img_brush = new ImageBrush();
                    img_brush.ImageSource = new BitmapImage(new System.Uri("pack://application:,,,/WpfApp1;component/Images/plus_blue_add.png"));
                    this.plus = img_brush;
                }
                this.BioPanel_btn.Background = plus;
            }
        }
    }
}
