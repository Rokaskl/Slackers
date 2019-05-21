using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using WpfApp1.Pages;
using System.Windows;
using System.Threading;

namespace WpfApp1
{
    class TcpDock
    {
        private event MyChangesEventHandler MessageChanged;
        private delegate void MyChangesEventHandler(object sender, ChangesEventArgs e);
        protected virtual void OnMessageChanged(ChangesEventArgs e)
        {
            MyChangesEventHandler handler = MessageChanged;
            handler?.Invoke(this, e);
            //if (MessageChanged != null)
            //{
            //    MessageChanged(this, e);
            //}
        }

        public class ChangesEventArgs : EventArgs
        {
            public int change_number;
        }

        private TcpClient client;

        public TcpDock(int port = 8080)
        {
            client = new TcpClient("localhost", port);
            MessageChanged += HandleChange;
            ConnectToServer();
        }

        private void HandleChange(object sender, ChangesEventArgs e)
        {
            switch (e.change_number)
            {
                case 0://Note
                    {
                        (Inst.Utils.RoomPage as RoomPage)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.RoomPage as RoomPage)?.UpdateNoteListView();
                        });
                        break;
                    }
                case 1://group chat
                    {
                        break;
                    }
                case 2://users listview
                    {
                        (Inst.Utils.RoomPage as RoomPage)?.UpdateUsersListView();
                        break;
                    }
                case 3://room members
                    {
                        
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }

        private void ConnectToServer()
        {
            byte[] buffer = BitConverter.GetBytes(1).Concat(BitConverter.GetBytes(int.Parse(Inst.Utils.User.id))).ToArray();//Encoding.UTF8.GetBytes("1" + Inst.Utils.User.id.ToString());//1 - last edit from wpf, 0 - last edit from webapi
            //buffer = (BitConverter.GetBytes(1) + BitConverter.GetBytes(int.Parse(Inst.Utils.User.id)));
            client.GetStream().Write(buffer, 0, buffer.Length);
            //client.GetStream().Read(buffer, 0, buffer.Length);

            Task.Run(() => Start(buffer));
            //Start();
            //char[] responsemsg = Encoding.UTF8.GetChars(buffer);
            //string rmsg = new string(responsemsg);
            //MessageBox.Show(rmsg);
            //client.GetStream().Close();
            //client.Close();
        }

        public void Stop()
        {
            Continue = false;
            client.GetStream().Close();
            client.Client.Close();
        }

        private bool Continue = true;

        public void Start(byte[] prevbuffer)
        {
            byte[] prev = prevbuffer;
            //while (true)
            //{
            //    try
            //    {
            //        if (client.GetStream().CanRead && client.Available == 16)
            //        {
            //            client.GetStream().Read(prev, 0, 16);
            //            break;
            //        }
            //    }
            //    catch(Exception exception)
            //    {

            //    }
            //}
            while (Continue)
            {
                try
                {
                    if (client.GetStream().CanRead && client.Available == 8)
                    {
                        byte[] buffer = new byte[8];
                        client.GetStream().Read(buffer, 0, 8);
                        if (!buffer.Equals(prev))
                        {
                            prev = buffer.ToArray();
                            OnMessageChanged(new ChangesEventArgs() { change_number = BitConverter.ToInt32(buffer, 4) });
                        }
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
