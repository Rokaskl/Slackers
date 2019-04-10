﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class Inst
    {
        public static Utils Utils;

        public static void CreateInstance()
        {
            Utils = new Utils();
        }
    }

    public class Utils
    {
        private HttpClient client;
        private Uri url;
        private string token;
        //private User user;

        public Utils()
        {
            this.client = new HttpClient();
            this.url = new Uri("http://localhost:4000");
            client.BaseAddress = this.url;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient HttpClient
        {
            get => client;
        }

        public Uri Url
        {
            get => url;
        }

        public string Token
        {
            get => token;
            set
            {
                this.token = value;
                this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + value);
            }
        }

        //public User User
        //{
        //    get => user;
        //}
    }

    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
    }
}