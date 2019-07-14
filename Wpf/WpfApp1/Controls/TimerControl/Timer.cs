using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp1.TimerControl
{
    class Timer : Label
    {
        private Stopwatch sw;
        private DispatcherTimer timer;
        private DateTime start;
        private DateTime stop;
        private bool running;
        //private TimeSpan timeSpan;
       
        public Timer()
        {
            //timeSpan = new TimeSpan();
            sw = new Stopwatch();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10000000);//10 000 000 ticks = 1 second.
            timer.Tick += Timer_Tick;
            this.Content = "00:00:00";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //timeSpan = timeSpan.Add(new TimeSpan(10000000));
            this.Content = sw.Elapsed.ToString("hh\\:mm\\:ss");
        }

        public TimeSpan Elapsed
        {
            get => sw.Elapsed;
        }

        public bool IsRunning
        {
            get => running;
        }

        public void Start()
        {
            start = DateTime.Now;
            timer.Start();
            sw.Start();
            this.running = true;
        }

        public void Stop()
        {
            stop = DateTime.Now;
            timer.Stop();
            sw.Stop();
            this.running = false;
        }

        public void Reset()
        {
            Stop();
            this.Content = "00:00:00";
        }
    }
}
