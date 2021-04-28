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
        const string URL = "https://www.playsport.cc/livescore.php?aid=2";
        List<string> outerGameboxIdList = new List<string>();

        string selectedGameboxA = "";
        string selectedGameboxB = "";
        Timer timerA = new Timer();
        Timer timerB = new Timer();
        string playerA1 = "";
        string playerA2 = "";
        string playerB1 = "";
        string playerB2 = "";


        public MainWindow()
        {
            InitializeComponent();

            SearchGame();

            timerA.Elapsed += TimerA_Elapsed;
            timerA.AutoReset = true;

            timerB.Elapsed += TimerB_Elapsed;
            timerB.AutoReset = true;
        }

        private async void SearchGame()
        {
            Log("正在從網頁取得資訊: " + URL);
            await Task.Delay(5);

            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(URL);
            try
            {
                List<string> vsList = new List<string>();

                foreach (HtmlNode outerGamebox in
                    htmlDocument.DocumentNode.SelectNodes("//*[@class='outer-gamebox']"))
                {
                    string gameboxId = outerGamebox.Id.Replace("outer-", string.Empty);

                    HtmlNode gamebox = outerGamebox.SelectSingleNode(
                        string.Format("descendant::*[@id='{0}']", gameboxId));

                    string awayTeam = gamebox.GetAttributeValue("data-namea", "錯誤");
                    string homeTeam = gamebox.GetAttributeValue("data-nameh", "錯誤");

                    if (!gamebox.InnerHtml.Contains("比賽結束"))
                    {
                        Log(awayTeam + " VS " + homeTeam);
                        vsList.Add(awayTeam + " VS " + homeTeam);

                        outerGameboxIdList.Add(outerGamebox.Id);
                    }
                }
                SelectGameComboBoxA.ItemsSource = vsList;
                SelectGameComboBoxB.ItemsSource = vsList;
            }
            catch
            {
                MessageBox.Show("對不起這個應用程式掛掉了QQ");
                Close();
            }
        }

        private void SelectGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ComboBox)sender == SelectGameComboBoxA)
            {
                selectedGameboxA = outerGameboxIdList[SelectGameComboBoxA.SelectedIndex];
                Log("選擇: " + SelectGameComboBoxA.SelectedItem);
            }
            else // (ComboBox)sender == SelectGameComboBoxB
            {
                selectedGameboxB = outerGameboxIdList[SelectGameComboBoxB.SelectedIndex];
                Log("選擇: " + SelectGameComboBoxB.SelectedItem);
            }
        }

        private void NotifyButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Button)sender == notifyButtonA)
            {
                if (notifyButtonA.Content.ToString() == "開啟提醒")
                {
                    playerA1 = playerA1textBox.Text;
                    playerA2 = playerA2textBox.Text;
                    timerA.Interval = intervalSlider.Value * 1000;
                    timerA.Enabled = true;
                    notifyButtonA.Content = "關閉提醒";
                    Log("已開啟提醒: " + playerA1 + " & " + playerA2);
                }
                else
                {
                    timerA.Enabled = false;
                    notifyButtonA.Content = "開啟提醒";
                    Log("已關閉提醒");
                }
            }
            else // (Button)sender == notifyButtonB
            {
                if (notifyButtonB.Content.ToString() == "開啟提醒")
                {
                    playerB1 = playerB1textBox.Text;
                    playerB2 = playerB2textBox.Text;
                    timerB.Interval = intervalSlider.Value * 1000;
                    timerB.Enabled = true;
                    notifyButtonB.Content = "關閉提醒";
                    Log("已開啟提醒: " + playerB1 + " & " + playerB2);
                }
                else
                {
                    timerB.Enabled = false;
                    notifyButtonB.Content = "開啟提醒";
                    Log("已關閉提醒");
                }
            }

        }

        private void TimerA_Elapsed(object sender, ElapsedEventArgs e)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(URL);
            try
            {
                HtmlNode gameBox = htmlDocument.DocumentNode.SelectSingleNode(
                    string.Format("//*[@id='{0}']", selectedGameboxA));

                HtmlNode inplay = gameBox.SelectSingleNode("descendant::*[@class='inplay-tr']");
                Console.WriteLine(inplay.InnerHtml);

                if (!string.IsNullOrEmpty(playerA1) && (inplay.InnerHtml.Contains(playerA1)))
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        timerA.Enabled = false;
                        notifyButtonA.Content = "開啟提醒";
                        Log("已關閉提醒");
                        Notify(playerA1);
                    });
                }
                else if (!string.IsNullOrEmpty(playerA2) && (inplay.InnerHtml.Contains(playerA2)))
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        timerA.Enabled = false;
                        notifyButtonA.Content = "開啟提醒";
                        Log("已關閉提醒");
                        Notify(playerA2);
                    });
                }
            }
            catch
            {
                MessageBox.Show("對不起這個應用程式掛掉了QQ");
                Close();
            }
        }

        private void TimerB_Elapsed(object sender, ElapsedEventArgs e)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(URL);
            try
            {
                HtmlNode gameBox = htmlDocument.DocumentNode.SelectSingleNode(
                    string.Format("//*[@id='{0}']", selectedGameboxB));

                HtmlNode inplay = gameBox.SelectSingleNode("descendant::*[@class='inplay-tr']");
                Console.WriteLine(inplay.InnerHtml);

                if (!string.IsNullOrEmpty(playerB1) && (inplay.InnerHtml.Contains(playerB1)))
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        timerB.Enabled = false;
                        notifyButtonB.Content = "開啟提醒";
                        Log("已關閉提醒");
                        Notify(playerB1);
                    });

                }
                else if (!string.IsNullOrEmpty(playerA2) && (inplay.InnerHtml.Contains(playerB2)))
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        timerB.Enabled = false;
                        notifyButtonB.Content = "開啟提醒";
                        Log("已關閉提醒");
                        Notify(playerB1);
                    });
                }
            }
            catch
            {
                MessageBox.Show("對不起這個應用程式掛掉了QQ");
                Close();
            }
        }

        private void Notify(string player)
        {
            this.Focus();
            this.Topmost = true;
            SystemSounds.Beep.Play();
            MessageBox.Show(player + " 上場");
            this.Topmost = false;
        }

        private void Log(string line)
        {
            logTextBlock.Text += line + Environment.NewLine;
        }
    }
}
