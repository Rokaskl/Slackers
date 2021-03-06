﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
    public class TcpServer
    {
        private TcpListener server;
        public TcpServer(int port = 10103)
        {
            server = new TcpListener(Dns.GetHostAddresses("localhost")[0], port);
            Start();
        }

        public TcpListener Listener
        {
            get => server;
            set => server = value;
        }

        private void Start()
        {
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
            }
            Task.Run(() => RunServer());
        }

        public bool SendInfo(int userId, string message_number, string text_data, int senderId)
        {
            UserInfo user = App.Inst.users.Find(x => x.id == userId);
            if (user != null)
            {
                byte[] buffer;

                if (!string.IsNullOrWhiteSpace(text_data))
                {
                   buffer = BitConverter.GetBytes(int.Parse(message_number)).Concat(BitConverter.GetBytes(senderId)).Concat(Encoding.UTF8.GetBytes(text_data)).ToArray();
                }
                else
                {
                    buffer = BitConverter.GetBytes(int.Parse(message_number)).ToArray();
                }
               
                
                while (true)
                {
                    try
                    {
                        if (user.Client.GetStream().CanWrite)
                        {
                            user.Client.GetStream().Write(buffer, 0, buffer.Length);
                            break;
                        }
                    }
                    catch(Exception exception)
                    {
                        break;
                    }
                } 
            }
            else
            {
                return false;
            }
            
            return true;
        }

        public async void RunServer()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                while (true)
                {
                    await Task.Delay(2);
                    if (client.GetStream().CanRead && client.Available >= 8)
                    {
                        byte[] buffer = new byte[client.Available];
                        client.GetStream().Read(buffer, 0, client.Available);
                        int userId = BitConverter.ToInt32(buffer, 4);
                        UserInfo user = App.Inst.users.FirstOrDefault(x => x.id == userId);
                        if (user != null)
                        {
                            user.Client = client;
                        }
                        break;
                    }
                }
            }
        }
    }
}
