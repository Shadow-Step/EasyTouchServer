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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using EasyTouchServer.serlib;

namespace EasyTouchServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TouchServer server;
        Thread server_thread;
        public MainWindow()
        {
            InitializeComponent();
            server = new TouchServer("192.168.1.33", 7434);
        }

        private void StartCloseServer(object sender, RoutedEventArgs e)
        {
            status_field.Content = "Online";
            server_thread = new Thread(server.Start);
            server_thread.Start();
        }
        private void CloseServer(object sender, RoutedEventArgs e)
        {
            status_field.Content = "Offline";
            server?.Close();
        }

        //TEMP!!!
        private void UpdateLog()
        {
            while (true)
            {
                if(Controller.log_changed)
                {
                    
                }
            }
        }
    }
}
