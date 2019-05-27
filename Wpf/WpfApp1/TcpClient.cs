﻿using System;
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
        }

        private TcpClient client;

        public TcpDock(int port = 6969)
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
                        (Inst.Utils.RoomPage as RoomPage)?.Dispatcher.Invoke(() =>
                        {
                            (Inst.Utils.RoomPage as RoomPage)?.UpdateGroupChat();
                        });
                        break;
                    }
                case 2://users listview
                    {
                        (Inst.Utils.RoomPage as RoomPage)?.UpdateUsersListView();
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
                                (Inst.Utils.RoomPage as RoomPage).Close();
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

        public void Start(byte[] prevbuffer)
        {
            byte[] prev = prevbuffer;

            while (Continue)
            {
                try
                {
                    if (client.GetStream().CanRead && client.Available >= 8)
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
