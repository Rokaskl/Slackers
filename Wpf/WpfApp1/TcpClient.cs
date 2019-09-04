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
using WpfApp1.Forms;

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
            public List<string> data;
        }

        private TcpClient client;

        public TcpDock(int port = 10103)
        {
            client = new TcpClient(Inst.Utils.Ip, port);
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
                        (Inst.Utils.RoomPage as RoomPage)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.RoomPage as RoomPage)?.UpdateGroupChat(e.data[1], e.data[0], e.data[2]);
                        });
                        break;
                    }
                case 2://users listview
                    {
                        (Inst.Utils.RoomPage as RoomPage)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.RoomPage as RoomPage)?.UpdateUsersListView(e.data[1], int.Parse(e.data[2]));
                        });
                        break;
                    }
                case 3://room members
                    {
                        (Inst.Utils.Administraktoring as Administraktoring)?.Dispatcher.Invoke(() =>
                        {
                            //Inst.Utils.RaiseMembersChangedEvent(this, EventArgs.Empty);//neveikia, nes webapi neranda prisijungusiu nariu ar roomu.
                            (Inst.Utils.Administraktoring as Administraktoring).UpdateMembersListView();
                        });
                        break;
                    }
                case 4://You got kicked out.
                    {
                        if(Inst.Utils.RoomPage != null)
                        {
                            //useris jau buna logoutintas is roomo...
                            (Inst.Utils.RoomPage as RoomPage)?.Dispatcher.Invoke(() =>
                            {
                                (Inst.Utils.RoomPage as RoomPage).CloseRoom();
                            });
                        }
                        
                        (Inst.Utils.UserPage as UserPage)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.UserPage as UserPage).UpdateRoomsListView();
                        });
                        
                        MessageBox.Show("You was kicked out!");
                        break;
                    }
                case 5://Room created, deleted, modified.
                    {
                        (Inst.Utils.AdminPage as Admin)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.AdminPage as Admin).UpdateRoomView();
                        });
                        break;
                    }
                case 6:
                    {
                        
                        Inst.Utils.MainWindow.Dispatcher.Invoke(() => 
                        {
                            Inst.Utils.MainWindow.HandleSignal_for_Fl_form(int.Parse(e.data[1]), int.Parse(e.data[2]));
                        });
                        break;   
                    }
                case 7:
                    {
                        Inst.Utils.MainWindow.Dispatcher.Invoke(() =>
                        {
                            Inst.Utils.MainWindow.HandleSignal_for_Fl_forms_friendschat_form(e.data[1], int.Parse(e.data[2]));
                        });
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
            byte[] buffer = BitConverter.GetBytes(1).Concat(BitConverter.GetBytes(int.Parse(Inst./*Utils*/ApiRequests.User.id))).ToArray();//1 - last edit from wpf, 0 - last edit from webapi
            client.GetStream().Write(buffer, 0, buffer.Length);

            Task.Run(() => Start(buffer));
        }

        public void Stop()
        {
            Continue = false;
            client.GetStream().Close();
            client.Client.Close();
        }

        private bool Continue = true;

        public async void Start(byte[] prevbuffer)
        {
            //byte[] prev = prevbuffer;

            while (Continue)
            {
                await Task.Delay(2);
                try
                {
                    if (client.GetStream().CanRead && client.Available >= 4)
                    {
                        byte[] buffer = new byte[client.Available];
                        int Available = client.Available;
                        client.GetStream().Read(buffer, 0, client.Available);
                        //if (!buffer.Equals(prev))
                        //{
                         //   prev = buffer.ToArray();
                            int change_number = BitConverter.ToInt32(buffer, 0);
                        ChangesEventArgs args = new ChangesEventArgs() { change_number = change_number, data = new List<string>() };
                        if (buffer.Length > 4)
                        {
                            int user_id = BitConverter.ToInt32(buffer, 4);
                            //string data = BitConverter.ToString(buffer, 8, Available - 8);
                            string data = Encoding.UTF8.GetString(buffer, 8, Available - 8);

                            args.data.Add(await Inst.Utils.GetUsername(user_id));//Reikia gauti user nickname is user id.
                            args.data.Add(data);
                            args.data.Add(user_id.ToString());
                        }
                        OnMessageChanged(args);
                        //}
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
