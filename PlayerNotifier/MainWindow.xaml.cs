using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayerNotifier
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer timer = new Timer();
        string url = "https://www.playsport.cc/livescore.php?aid=2";
        string player = "";
        string playerPrev = "";

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = 15 * 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //MessageBox.Show("times up");
            HtmlWeb web = new HtmlWeb();
            web.UsingCache = false;
            HtmlDocument doc = web.Load(url);
            try
            {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id='outer-gamebox-1738699']");
                node = node.SelectSingleNode("descendant::*[@class='inplay-tr']");
                Console.WriteLine(node.InnerHtml);
                if (!string.IsNullOrEmpty(player) && node.InnerHtml.Contains(player))
                {
                    this.Dispatcher.InvokeAsync(() => Notify(player));
                }
                else if (!string.IsNullOrEmpty(playerPrev) && node.InnerHtml.Contains(playerPrev))
                {
                    this.Dispatcher.InvokeAsync(() => Notify(playerPrev));
                }
            }
            catch
            {
                Console.WriteLine("parse fail");
            }
}

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (button.Content.ToString() == "Start")
            {
                player = textBox.Text;
                playerPrev = textBoxPrev.Text;
                timer.Enabled = true;
                button.Content = "Stop";
            }
            else
            {
                timer.Enabled = false;
                button.Content = "Start";
            }
        }

        private void Notify(string player)
        {
            this.Focus();
            this.Topmost = true;
            SystemSounds.Beep.Play();
            MessageBox.Show(player + " At Bat");
            this.Topmost = false;
        }
    }
}
