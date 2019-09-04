using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Controls.Chat;
using WpfApp1.ViewModels;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for FriendsChatForm.xaml
    /// </summary>
    public partial class FriendsChatForm : Window
    {
        public List<TabItem> Tabs { get; set; }

        public FriendsChatForm()
        {
            InitializeComponent();
            Tabs = new List<TabItem>();
        }

        public void FocusTab(int id)
        {
            for (int i = 0; i < this.chat_tab_ctrl.Items.Count; i++)
            {
                if((((this.chat_tab_ctrl.Items[i] as TabItem).Content as ChatControl).DataContext as ChatViewModel).Id == id)
                {
                    this.chat_tab_ctrl.SelectedItem = this.chat_tab_ctrl.Items[i];
                    break;
                }
            }
            
        }

        private void btn_TabClose_Click(object sender, RoutedEventArgs e)
        {
            RemoveTab((((sender as Button).Parent as Grid).DataContext as ChatViewModel));
        }

        public void AddTab(ChatViewModel chat_vm)
        {
            TabItem new_tab = new TabItem();
            ChatControl chat_ctrl = new ChatControl();
            chat_ctrl.DataContext = chat_vm;
            chat_ctrl.Margin = new Thickness(0, 6, 0, 0);
            new_tab.Content = chat_ctrl;
            new_tab.DataContext = chat_vm;
            this.Tabs.Add(new_tab);
            this.chat_tab_ctrl.Items.Add(new_tab);
            this.chat_tab_ctrl.SelectedItem = new_tab;
            new_tab.Loaded += New_tab_Loaded;
        }

        private void New_tab_Loaded(object sender, RoutedEventArgs e)
        {
            ((sender as TabItem).GetChildOfType<WpfApp1.Controls.UsersList.UsersListLineControl>() as WpfApp1.Controls.UsersList.UsersListLineControl).BioPanel_btn.Visibility = Visibility.Hidden;
        }

        public void RemoveTab(int id)
        {
            TabItem tab_to_remove = Tabs.FirstOrDefault(x => ((x.Content as ChatControl).DataContext as ChatViewModel).Id == id);

            if (tab_to_remove == null)
            {
                return;
            }
            tab_to_remove.Loaded -= New_tab_Loaded;
            this.chat_tab_ctrl.Items.Remove(tab_to_remove);
            this.Tabs.Remove(tab_to_remove);

            if (this.chat_tab_ctrl.Items.Count == 0)
            {
                this.Close();
            }
        }

        public void RemoveTab(ChatViewModel chat_vm)
        {
            (this.DataContext as FriendsChatFormViewModel).RemoveChat(chat_vm);
            RemoveTab(chat_vm.Id);
        }
    }
}
