using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for LogsForm.xaml
    /// </summary>
    public partial class LogsForm : Window
    {

        private ObservableCollection<LogLine> log_lines;
        public ObservableCollection<LogLine> LogLines
        {
            get
            {
                return log_lines;
            }
            set
            {
                log_lines = value;
            }
        }

        public LogsForm()
        {
            InitializeComponent();
            this.LogTab_ScrollViewer.ScrollChanged += LogTab_ScrollViewer_ScrollChanged;
            this.Closing += LogsForm_Closing;
            this.LogLines = new ObservableCollection<LogLine>();
            
            LoadLog();
        }

        private void LogsForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Inst.Utils.MainWindow.Logs_form = null;
        }

        private void LogTab_ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scv = (sender as ScrollViewer);
            if (finished_loading && scv.VerticalOffset == scv.ScrollableHeight)
            {
                LoadLog();
            }
        }

        private bool finished_loading;
        private int current_log_page = 0;
        private int log_count_per_page = 20;

        private async void LoadLog()
        {
            List<LogLine> logs = await Inst.ApiRequests.GetLogLines(current_log_page * log_count_per_page, log_count_per_page);
            if (logs.Count > 0)
            {
                current_log_page++;
                logs.ForEach(async x => await Inst.Utils.PopulateLogLinesWithNames(x));
                logs.ForEach(x => this.LogLines.Add(x));
                this.DataContext = this.LogLines;
            }
            finished_loading = true;
            //this.UpdateLayout();
        }
    }
}
