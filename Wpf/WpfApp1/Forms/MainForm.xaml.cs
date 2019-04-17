﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Json.Net;
using WebApi.Dtos;
using Newtonsoft.Json;
using WpfApp1.Forms;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private HttpClient client;
        public MainForm()
        {
            Inst.CreateInstance();
            client = Inst.Utils.HttpClient;
            LoginForm loginForm = new LoginForm();
            if (!(loginForm.ShowDialog() ?? false))
            {
                //Application.Exit
                //Environment.Exit
                this.Close();
                return;
            }
            InitializeComponent();
            frame.NavigationService.Navigate(new RoomsPage());
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
    }
}